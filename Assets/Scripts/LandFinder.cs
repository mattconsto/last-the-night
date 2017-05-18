using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFinder : MonoBehaviour {
	public Vector3 offset = new Vector3(0, 0, 0);

	public void Update() {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -Vector3.up, out hit) && hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.name != "Water") {
			Debug.Log("Found land @ " + hit.point);
			transform.position = hit.point + Vector3.up * 10 + offset;
			enabled = false;
		}
	}
}
