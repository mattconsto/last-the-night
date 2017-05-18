using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	private GameController _controller;
	public float difficulty;

	public void Start() {
		_controller = FindObjectOfType<GameController>();
	}

	public void OnPointerClick(PointerEventData ed) {
		// Start the game with selected difficulty
		_controller.Intro(difficulty);
	}

	public void OnSubmit(BaseEventData ed) {
		_controller.Intro(difficulty);
	}
}
