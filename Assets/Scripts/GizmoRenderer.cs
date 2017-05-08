using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Draw a Gizmo
*/
public class GizmoRenderer : MonoBehaviour {
	public string gizmoPath = "";

	public void OnDrawGizmos() {
		if(gizmoPath != "") Gizmos.DrawIcon(transform.position, gizmoPath, true);
	}
}
