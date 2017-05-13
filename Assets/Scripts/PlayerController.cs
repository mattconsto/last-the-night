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

	public void Start () {
		_rb = GetComponent<Rigidbody>();
	}

    public void FixedUpdate() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Running
        float multiplier = Input.GetButton("Fire3") ? 2 : 1;
        if(Input.GetButton("Fire3")) {
            multiplier = 2;
            runningSource.volume = Mathf.Lerp(runningSource.volume, 0.75f, 0.005f);
        } else {
            runningSource.volume = Mathf.Lerp(runningSource.volume, 0, 0.1f);
        }

        // Footsteps
        if(_canJump > 0) {
            _distance += (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * multiplier;
            if(_distance > 50) {
                effectSource.PlayOneShot(stepClip, 0.25f);
                _distance = 0;
            }
        }

		_rb.AddForce(transform.right * Input.GetAxis("Horizontal") * speed * multiplier, ForceMode.Impulse);
		_rb.AddForce(transform.forward * Input.GetAxis("Vertical") * speed * multiplier, ForceMode.Impulse);
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);
		cam.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * sensitivity);
        cam.transform.localEulerAngles = new Vector3((Mathf.Clamp((cam.transform.localEulerAngles.x + 90) % 360, 50, 130) + 270) % 360, 0, 0);

        if(Input.GetButton("Jump") && _canJump > 0) {
            _rb.AddForce(transform.up * jump);
            effectSource.PlayOneShot(jumpClip, 0.5f);
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

    public void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Jumpable") _canJump++;
    }

    public void OnCollisionExit(Collision col) {
        if(col.gameObject.tag == "Jumpable") _canJump--;
    }
}