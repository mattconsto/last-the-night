using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Color hoverColor = Color.white;
	private Color _defaultColor = Color.black;

	public void Start() {
		_defaultColor = GetComponent<Text>().color;
	}

	public void OnPointerEnter(PointerEventData ed) {
		GetComponent<Text>().color = hoverColor;
	}

	public void OnPointerExit(PointerEventData ed) {
		GetComponent<Text>().color = _defaultColor;
	}
}
