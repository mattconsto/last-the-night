using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MessageAction {
	NONE = 0, CHOICE = 1, END = 2, KILL = 3, FINISH = 4, SUICIDE = 5, JOIN = 6
}

[System.Serializable]
public class Message {
	public string text = "";
	public AudioClip clip;
	public MessageAction action;
	public GameObject target;
	public Message[] children;
}

public class Dialogue : MonoBehaviour {
	//public GameController controller;
	public Message root;
	public int uses = -1;

	public void OnTriggerEnter(Collider col) {
		if(uses != 0 && col.gameObject.tag == "Player" && !col.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled) {
			//controller.BeginDialogue(root);
			if(uses > 0) uses--;
		}
	}
}
