using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	public GameController controller;

	public void OnPointerClick(PointerEventData ed) {
		controller.Begin();
	}

	public void OnSubmit(BaseEventData ed) {
		controller.Begin();
	}
}
