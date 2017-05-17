using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFinder : MonoBehaviour {
	public void Update() {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, -Vector3.up, out hit) && hit.collider.gameObject.tag != "Player") {
            	Debug.Log("Found land @ " + hit.point);
                transform.position = hit.point + Vector3.up * 10;
                enabled = false;
            }
	}
}
