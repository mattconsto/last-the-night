using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	private HUDController hud;
	public PlayerController player;

	public enum State {MENU, PLAY, PAUSE, END};
	public State state = State.MENU;

	public void Start() {
		hud = GetComponent<HUDController>();
	}

	public void Begin() {
		if(state != State.PLAY) {
			state = State.PLAY;
			hud.ToggleHUDs();
			player.enabled = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	public void End() {
		if(state != State.END) {
			state = State.END;
			hud.ToggleHUDs();
			player.enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
