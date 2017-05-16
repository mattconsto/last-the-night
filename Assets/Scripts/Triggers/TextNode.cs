using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNode : MonoBehaviour {
	protected HUDController _hud;

	public string text = "";
	public string hint = "";
	public AudioClip clip;
	public float  time = 1;
	public int    uses = -1;

	public void Start() {
		_hud = FindObjectOfType<HUDController>();
	}

	public void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Player" && uses != 0) {
			if(text != "") _hud.SetSubtitle(text, time);
			if(hint != "") _hud.SetHint(hint);
			if(clip != null && GetComponent<AudioSource>() != null) GetComponent<AudioSource>().PlayOneShot(clip, 1);
			if(uses > 0) uses--;
		}
	}
}
