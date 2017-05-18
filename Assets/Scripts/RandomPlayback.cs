using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayback : MonoBehaviour {
	public float fadeTime = 1f;
	public bool random = true;

	private float _timer = 0;
	private AudioSource _source;

	public void Start() {
		_source = GetComponent<AudioSource>();

		// Start the track from a random place
		if(_source != null) {
			if(random) _source.time = new System.Random().NextFloat(_source.clip.length);
			_source.volume = 0;
			_source.Play();
			_timer = fadeTime;
		}
	}

	public void Update() {
		// Fade in
		_timer -= Time.deltaTime;
		if(_source != null) _source.volume = 1 - _timer / fadeTime;
		if(_timer < 0) enabled = false;
	}
}
