using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNode : MonoBehaviour {
	//public GameController controller;

	public string text = "";
	public string hint = "";
	public AudioClip clip;
	public float  time = 1;
	public int    uses = -1;

	public void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Player" && !col.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled && uses != 0) {
			//if(text != "") controller.SetMessage(text, time);
			//if(hint != "") controller.SetHint(hint, time);
			//if(clip != null) controller.PlayNaration(clip);
			if(uses > 0) uses--;
		}
	}
}
