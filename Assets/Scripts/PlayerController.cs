using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public HUDController hud;

	public AudioSource runningSource;
	public AudioSource effectSource;

	public AudioClip jumpClip;
	public AudioClip stepClip;
	public AudioClip torchClip;

	private Rigidbody _rb;

	public float speed = 1;
	public float jump = 100;

	public GameObject cam;
	public GameObject torch;
	public GameObject Hand;

	public float _distance = 0;
	public float _totalDistance = 0;
	private float _multiplier = 1;
	private float _jumpTimer = 0;

	public List<GameObject> weapons = new List<GameObject>();
	public int selectedWeapon = 0;

	public float sensitivityX = 2F;
	public float sensitivityY = 2F;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	private float rotationY = 0F;

	public void Start () {
		_rb = GetComponent<Rigidbody>();
		Transform hand = Hand.transform;
		for (int i = 0; i < weapons.Count; i++) {
			Debug.Log(weapons[0]);
			weapons[i] = Instantiate(weapons[i], hand.transform.position, hand.transform.rotation);
			weapons[i].SetActive(false);
			weapons[i].transform.parent = hand;
		}
		weapons[selectedWeapon].SetActive(true);
	}

	public void FixedUpdate() {
		_jumpTimer -= Time.deltaTime;

		// Footsteps
		float temp = (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) * _multiplier;
		_distance += temp;
		_totalDistance += temp;
		if(_distance > 50) {
			effectSource.PlayOneShot(stepClip, 0.25f);
			_distance = 0;
		}

		_rb.MovePosition(transform.position + transform.forward * Input.GetAxis("Vertical") * speed * _multiplier + transform.right * Input.GetAxis("Horizontal") * speed * _multiplier);

		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

		if(Input.GetButton("Jump") && _jumpTimer < 0) {
			RaycastHit hit;
			if(Physics.Raycast(transform.position, -transform.up, out hit) && hit.distance < 3) {
				_rb.AddForce(transform.up * jump);
				effectSource.PlayOneShot(jumpClip, 0.5f);
				_jumpTimer = 1;
			}
		}
	}

	public void Update() {
		// Running
		if(Input.GetButton("Fire3")) {
			GetComponent<Destructable>()._staminaTimer = 2f;
			if(GetComponent<Destructable>().stamina > 0) {
				_multiplier = 2;
				runningSource.volume = Mathf.Lerp(runningSource.volume, 0.75f, 0.005f);
				GetComponent<Destructable>().stamina -= Time.deltaTime;
			} else {
				_multiplier = 1;
				runningSource.volume = Mathf.Lerp(runningSource.volume, 0, 0.1f);
			}
		} else {
			_multiplier = 1;
			runningSource.volume = Mathf.Lerp(runningSource.volume, 0, 0.1f);
		}

		if(Input.GetButton("Fire1")) {
			WeaponController controller = weapons[selectedWeapon].GetComponent<WeaponController>();
			if(controller != null) controller.Fire();
		}

		if(Input.GetButtonDown("Submit")) {
			torch.SetActive(!torch.activeSelf);
			effectSource.PlayOneShot(torchClip, 0.5f);
		}

		hud.SetHealth(GetComponent<Destructable>().health/GetComponent<Destructable>().maxHealth);
		hud.SetStamina(GetComponent<Destructable>().stamina/GetComponent<Destructable>().maxStamina);
	}
}
