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

	public void Start() {
		_start  = sun.transform.eulerAngles;
	}

	public void Update() {
		sun.transform.eulerAngles = _start + multiplier * player._totalDistance;
		time = Mathf.Sin((sun.transform.eulerAngles.x % 360) * Mathf.Deg2Rad);
		RenderSettings.fogColor = Color.Lerp(darkFog, lightFog, time);

		// Praise the sun
		if(time > 0.95f) {
			Debug.Log("You win!");
			GetComponent<GameController>().Win();
		}
	}
}
