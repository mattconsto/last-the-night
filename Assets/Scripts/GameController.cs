using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	private HUDController hud;
	public PlayerController player;
	public InfiniteTerrain generator;
	public Text seed;

	public enum State {MENU, PLAY, PAUSE, END};
	public State state = State.MENU;

	public void Start() {
		hud = GetComponent<HUDController>();
	}

	public void Update() {
		if(Input.GetButtonDown("Cancel")) {
			if(state == State.PLAY) {
				Pause();
			} else if(state == State.PAUSE) {
				Resume();
			}
		}

		if(!player.gameObject.activeSelf) {
			End();
		}
	}

	public void Begin() {
		if(state != State.PLAY) {
			hud.ToggleHUDs();
			player.enabled = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			generator.config.seed = seed.text.Length > 0 ? Convert.ToInt32(seed.text) : 0;
			generator.enabled = true;
			player.GetComponent<Rigidbody>().isKinematic = false;
			transform.Find("HUDs/Title HUD/Menu/Start").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(true);
			state = State.PLAY;
		}
	}

	public void Pause() {
		if(state != State.PAUSE) {
			hud.ToggleHUDs();
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 0;
			state = State.PAUSE;
		}
	}

	public void Resume() {
		if(state != State.PLAY) {
			hud.ToggleHUDs();
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			Time.timeScale = 1;
			state = State.PLAY;
		}
	}

	public void End() {
		if(state != State.END) {
			hud.ToggleHUDs();
			player.enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			generator.enabled = false;
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Restart").gameObject.SetActive(true);
			state = State.END;
		}
	}
}
