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

	public enum State {MENU, INTRO, PLAY, PAUSE, END, WIN};
	public State state = State.MENU;

	public string introText;
	public AudioClip introClip;
	public AudioSource introSource;

	private float _introTimer = 0;

	public void Start() {
		hud = GetComponent<HUDController>();
	}

	public void Update() {
		_introTimer -= Time.deltaTime;

		if(_introTimer < 0 && state == State.INTRO) {
			Begin();
		}

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

	public void Into() {
		if(state != State.INTRO) {
			player.cam.SetActive(true);
			hud.ToggleHUDs(false);
			player.enabled = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			generator.config.seed = seed.text.Length > 0 ? Convert.ToInt32(seed.text) : 0;
			generator.enabled = true;
			player.GetComponent<Rigidbody>().isKinematic = false;
			Time.timeScale = 0;
			transform.Find("HUDs/Title HUD/Menu/Start").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(true);
			_introTimer = introClip != null ? introClip.length + 1 : 5;
			hud._blackTime = _introTimer;
			hud.SetSubtitle(introText, _introTimer);
			if(introClip != null && introSource != null) introSource.PlayOneShot(introClip, _introTimer);
			state = State.INTRO;
		}
	}

	public void Begin() {
		if(state != State.PLAY) {
			player.cam.SetActive(true);
			hud.ToggleHUDs(false);
			player.enabled = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			generator.config.seed = seed.text.Length > 0 ? Convert.ToInt32(seed.text) : 0;
			Time.timeScale = 1;
			generator.enabled = true;
			player.GetComponent<Rigidbody>().isKinematic = false;
			transform.Find("HUDs/Title HUD/Menu/Start").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(true);
			state = State.PLAY;
		}
	}

	public void Pause() {
		if(state != State.PAUSE) {
			hud.ToggleHUDs(true);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 0;
			state = State.PAUSE;
		}
	}

	public void Resume() {
		if(state != State.PLAY) {
			hud.ToggleHUDs(false);
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			Time.timeScale = 1;
			state = State.PLAY;
		}
	}

	public void End() {
		if(state != State.END) {
			hud.ToggleHUDs(true);
			hud._hurtTime = Mathf.Infinity;
			player.enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			generator.enabled = false;
			Time.timeScale = 0;
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Restart").gameObject.SetActive(true);
			state = State.END;
		}
	}

	public void Win() {
		if(state != State.WIN) {
			hud.ToggleHUDs(true);
			hud._hurtTime = 0;
			player.enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			generator.enabled = false;
			Time.timeScale = 0;
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Restart").gameObject.SetActive(true);
			state = State.END;
		}
	}
}
