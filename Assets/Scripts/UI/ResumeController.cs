using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	private GameController _controller;

	public void Start() {
		_controller = FindObjectOfType<GameController>();
	}

	public void OnPointerClick(PointerEventData ed) {
		_controller.Resume();
	}

	public void OnSubmit(BaseEventData ed) {
		_controller.Resume();
	}
}
