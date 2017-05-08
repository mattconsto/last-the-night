using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicTrigger : MonoBehaviour {
	public GameObject[] objects;
	public float impulse;

	private bool moved = false;

	public void OnCollisionEnter(Collision col) {
		// Start rotating
		if(!moved && (objects.Length == 0 || System.Array.IndexOf(objects, col.gameObject) != -1) && col.impulse.magnitude >= impulse) {
			GetComponent<Rigidbody>().isKinematic = false;
			moved = true;
		}
	}
}
