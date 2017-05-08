using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTrigger : MonoBehaviour {
	public float time;
	private float timer = 0;
	public Vector3 degrees;
	public Vector3 movement;

	public AudioClip clip;
	public AudioSource source;

	public GameObject[] objects;
	public float impulse;

	private bool moved = false;

	public void FixedUpdate() {
		if(timer > 0) {
			timer -= Time.deltaTime;
			transform.Rotate(Time.deltaTime / time * degrees);
			transform.Translate(Time.deltaTime / time * movement);
		} else {
			// We don't want to do unnecessary work.
			enabled = false;
		}
	}

	public void OnCollisionEnter(Collision col) {
		// Start rotating
		if(!moved && (objects.Length == 0 || System.Array.IndexOf(objects, col.gameObject) != -1) && col.impulse.magnitude >= impulse) {
			if(source != null && clip != null) source.PlayOneShot(clip, 1);
			timer = time;
			moved = true;
			enabled = true;
		}
	}
}
