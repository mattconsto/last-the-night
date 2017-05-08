using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalSoundSource : MonoBehaviour, IPointerClickHandler {
	public GameObject controller;
	public AudioClip  clip;
	public float      volume = 1f;

	public void OnPointerClick(PointerEventData ed) {
		controller.GetComponent<AudioSource>().PlayOneShot(clip, volume);
	}
}
