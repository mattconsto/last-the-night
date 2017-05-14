using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public AudioSource runningSource;
    public AudioSource effectSource;

    public AudioClip jumpClip;
    public AudioClip stepClip;
    public AudioClip torchClip;

	private Rigidbody _rb;
    private float _canJump = 0;

	public float speed = 1;
    public float jump = 100;
	public float sensitivity = 2;

	public GameObject cam;
	public GameObject torch;

    private float _distance;
    private float _multiplier = 1;

    public List<GameObject> weapons = new List<GameObject>();
    public int selectedWeapon = 0;

	public void Start () {
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
        // Footsteps
        if(_canJump > 0) {
            _distance += (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * _multiplier;
            if(_distance > 50) {
                effectSource.PlayOneShot(stepClip, 0.25f);
                _distance = 0;
            }
        }

        _rb.AddForce(transform.right * Input.GetAxis("Horizontal") * speed * _multiplier, ForceMode.Impulse);
        _rb.AddForce(transform.forward * Input.GetAxis("Vertical") * speed * _multiplier, ForceMode.Impulse);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);

        if(Input.GetButton("Jump") && _canJump > 0) {
            _rb.AddForce(transform.up * jump);
            effectSource.PlayOneShot(jumpClip, 0.5f);
        }
    }

    public void Update() {
        // Running
        if(Input.GetButton("Fire3")) {
            _multiplier = 2;
            runningSource.volume = Mathf.Lerp(runningSource.volume, 0.75f, 0.005f);
        } else {
            _multiplier = 1;
            runningSource.volume = Mathf.Lerp(runningSource.volume, 0, 0.1f);
        }

        if(Input.GetButtonDown("Submit")) {
            torch.SetActive(!torch.activeSelf);
            effectSource.PlayOneShot(torchClip, 0.5f);
        }

        #if !UNITY_EDITOR
        if (Input.GetButtonDown("Cancel")) {
	       Application.Quit();
        }
        #endif
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
        cam.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * sensitivity);
        cam.transform.localEulerAngles = new Vector3((Mathf.Clamp((cam.transform.localEulerAngles.x + 90) % 360, 50, 130) + 270) % 360, 0, 0);
    }

    public void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Jumpable") _canJump++;
    }

    public void OnCollisionExit(Collision col) {
        if(col.gameObject.tag == "Jumpable") _canJump--;
    }
}
