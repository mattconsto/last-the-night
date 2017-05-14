using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	public GameController controller;

	public void OnPointerClick(PointerEventData ed) {
		controller.Resume();
	}

	public void OnSubmit(BaseEventData ed) {
		controller.Resume();
	}
}
