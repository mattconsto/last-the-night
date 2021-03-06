﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	public GameObject[] targets;

	public void OnPointerClick(PointerEventData ed) {
		// Toggle visibilty, and then select a target.
		foreach(GameObject target in targets) target.SetActive(!target.activeSelf);
		foreach(GameObject target in targets) {
			if(target.activeInHierarchy && target.GetComponent<Selectable>() != null) {
				target.GetComponent<Selectable>().Select();
				break;
			}
		}
	}

	public void OnSubmit(BaseEventData ed) {
		foreach(GameObject target in targets) target.SetActive(!target.activeSelf);
		foreach(GameObject target in targets) {
			if(target.activeInHierarchy && target.GetComponent<Selectable>() != null) {
				target.GetComponent<Selectable>().Select();
				break;
			}
		}
	}
}
