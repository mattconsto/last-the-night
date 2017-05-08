using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadTrigger : MonoBehaviour {
	public float force = 1;

	public void OnTriggerEnter(Collider col) {
			col.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * force);
			if(GetComponent<AudioSource>() != null) GetComponent<AudioSource>().Play();
	}
}
