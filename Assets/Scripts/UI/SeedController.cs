using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedController : MonoBehaviour, ISubmitHandler {
	public GameController controller;

	private InputField _field;

	public void Start() {
		_field = GetComponent<InputField>();
		int seed = new System.Random().Next(0, 10000000);
		Debug.Log("Seed: " + seed);
		GetComponent<InputField>().text = "" + seed;
	}

	public void Update() {
		if(_field.isFocused && _field.text != "" && Input.GetKey(KeyCode.Return)) {
			controller.Begin();
			enabled = false;
		}
	}
	
	public void OnSubmit(BaseEventData ed) {
		controller.Begin();
	}
}
