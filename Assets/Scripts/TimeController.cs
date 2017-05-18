using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {
	public GameObject sun;
	public Vector3 multiplier = new Vector3(0, 0, 0);
	public PlayerController player;
	public float time;

	public Color lightFog;
	public Color darkFog;

	private Vector3 _start;
	private GameController _controller;

	public void Start() {
		_start  = sun.transform.eulerAngles;
		_controller = GetComponent<GameController>();
	}

	public void Update() {
		// Change time as player moves
		sun.transform.eulerAngles = _start + multiplier * player._totalDistance;
		time = Mathf.Sin((sun.transform.eulerAngles.x % 360) * Mathf.Deg2Rad);
		RenderSettings.fogColor = Color.Lerp(darkFog, lightFog, Mathf.Clamp01(time * 10 - 2));

		// Praise the sun
		if(time > 0.60f && (_controller.state != GameController.State.WIN && _controller.state != GameController.State.WINPAUSE)) {
			Debug.Log("You win!");
			_controller.Win();
		}
	}
}
