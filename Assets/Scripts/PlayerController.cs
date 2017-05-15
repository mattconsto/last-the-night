using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public HUDController hud;
    public GameController controller;

    public AudioSource runningSource;
    public AudioSource effectSource;

    public AudioClip jumpClip;
    public AudioClip stepClip;
    public AudioClip torchClip;

	private Rigidbody _rb;

	public float speed = 1;
    public float jump = 100;
	public float sensitivity = 2;

	public GameObject cam;
	public GameObject torch;

    public float _distance = 0;
    public float _totalDistance = 0;
    private float _multiplier = 1;
    private float _jumpTimer = 0;

    public List<GameObject> weapons = new List<GameObject>();
    public int selectedWeapon = 0;

	public void Start() {
		_rb = GetComponent<Rigidbody>();
        Transform hand = transform.Find("Hand");
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

        _rb.MovePosition(transform.position + transform.right * Input.GetAxis("Horizontal") * speed * _multiplier);
        _rb.MovePosition(transform.position + transform.forward * Input.GetAxis("Vertical") * speed * _multiplier);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);

        if(Input.GetButton("Jump") && _jumpTimer < 0) {
            // Are we on the ground?
            RaycastHit hit;
            if(Physics.Raycast(transform.position, -transform.up, out hit) && hit.distance < 3) {
                _rb.AddForce(transform.up * jump);
                effectSource.PlayOneShot(jumpClip, 0.5f);
                _jumpTimer = 1;
            }
        }
    }

    public void Update() {
        // If we fall out of the level
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.up, out hit) && hit.transform.gameObject.name == "Water" && hit.transform.gameObject.name == "Terrain Chunk") {
            Debug.Log("Found: " + hit.transform.gameObject.name);
            transform.position = hit.point + new Vector3(0, 10, 0);
        }

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

        // Torch
        if(Input.GetButtonDown("Submit")) {
            torch.SetActive(!torch.activeSelf);
            effectSource.PlayOneShot(torchClip, 0.5f);
        }

        hud.SetHealth(GetComponent<Destructable>().health/GetComponent<Destructable>().maxHealth);
        hud.SetStamina(GetComponent<Destructable>().stamina/GetComponent<Destructable>().maxStamina);
	}

    /* Event Listeners*/

	public void OnFire(float value) {
		// if(_respawnTimer > 0) return;

		if(value > 0) {
			WeaponController controller = weapons[selectedWeapon].GetComponent<WeaponController>();
			if(controller != null) controller.Fire();
		}
	}

	public void OnSwitch(int value) {
		weapons[selectedWeapon].SetActive(false);
		selectedWeapon = (selectedWeapon + value + weapons.Count) % weapons.Count;
		weapons[selectedWeapon].SetActive(true);
		// Set message here
		// Play audio here
	}

	public void AddWeapon(GameObject prefab) {
		Transform hand = transform.Find("Hand");

		for(int i = 0; i < weapons.Count; i++) weapons[i].SetActive(false);

		weapons.Add(Instantiate(prefab, hand.transform.position, hand.transform.rotation));
		weapons[weapons.Count -1].transform.parent = hand;
		selectedWeapon = weapons.Count - 1;

		// Set Weapon Message healthRegen
		// Play Switch weapon audio
	}

	// public void OnReload() {
	// 	weapons[selectedWeapon].GetComponent<WeaponController>().Reload();
	// }

    public void LateUpdate() {
        if(controller.state == GameController.State.PLAY || controller.state == GameController.State.WIN) {
            cam.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * sensitivity);
            cam.transform.localEulerAngles = new Vector3((Mathf.Clamp((cam.transform.localEulerAngles.x + 90) % 360, 50, 130) + 270) % 360, 0, 0);
        }
    }
}
