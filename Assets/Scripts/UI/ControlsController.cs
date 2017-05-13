using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	public GameObject[] targets;

	public void OnPointerClick(PointerEventData ed) {
		foreach(GameObject target in targets) target.SetActive(!target.activeSelf);
	}

	public void OnSubmit(BaseEventData ed) {
		foreach(GameObject target in targets) target.SetActive(!target.activeSelf);
	}
}
