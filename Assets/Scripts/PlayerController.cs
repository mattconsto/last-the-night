using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody _rb;
    private float _canJump = 0;

	public float speed = 1;
    public float jump = 100;
	public float sensitivity = 2;

	public GameObject cam;
	public GameObject torch;

	public void Start () {
		_rb = GetComponent<Rigidbody>();
	}

    public void FixedUpdate() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

		_rb.MovePosition(_rb.position + transform.right * Input.GetAxis("Horizontal") * speed);
		_rb.MovePosition(_rb.position + transform.forward * Input.GetAxis("Vertical") * speed);
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);
		cam.transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * sensitivity);
        cam.transform.localEulerAngles = new Vector3((Mathf.Clamp((cam.transform.localEulerAngles.x + 90) % 360, 50, 130) + 270) % 360, 0, 0);

        if(Input.GetButton("Jump") && _canJump > 0) {
            _rb.AddForce(transform.up * jump);
        }

        if(Input.GetButtonDown("Submit")) {
            torch.SetActive(!torch.activeSelf);
        }

        if (Input.GetButtonDown("Cancel")) {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
			            Application.Quit();
            #endif
        }
	}

    public void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag == "Jumpable") _canJump++;
    }

    public void OnCollisionExit(Collision col) {
        if(col.gameObject.tag == "Jumpable") _canJump--;
    }
}