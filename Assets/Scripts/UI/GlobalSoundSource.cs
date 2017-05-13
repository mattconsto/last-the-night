using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalSoundSource : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	public AudioSource source;
	public AudioClip   clip;
	public float       volume = 1f;

	public void OnPointerClick(PointerEventData ed) {
		source.PlayOneShot(clip, volume);
	}

	public void OnSubmit(BaseEventData ed) {
		source.PlayOneShot(clip, volume);
	}
}
