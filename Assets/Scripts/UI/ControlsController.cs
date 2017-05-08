using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsController : MonoBehaviour, IPointerClickHandler {
	public GameObject target;
	public GameObject hidden;

	public void OnPointerClick(PointerEventData ed) {
		OnToggle();
	}

	public void OnToggle() {
		target.SetActive(!target.activeSelf);
		hidden.SetActive(!target.activeSelf);
	}
}
