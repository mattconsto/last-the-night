﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	private HUDController hud;
	public PlayerController player;
	public InfiniteTerrain generator;
	public Text seed;

	public enum State {MENU, INTRO, PLAY, PAUSE, END, WIN, WINPAUSE};
	public State state = State.MENU;
	public float difficulty = 1;

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
			Begin(difficulty);
		}

		if(Input.GetButtonDown("Cancel")) {
			switch(state) {
				case State.PLAY:     Pause(); break;
				case State.PAUSE:    Resume(); break;
				case State.WIN:      WinPause(); break;
				case State.WINPAUSE: Win(); break;
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
			hud.SetSeed(generator.config.seed);
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

	public void Begin(float difficulty) {
		if(state != State.PLAY) {
			this.difficulty = difficulty;
			player.cam.SetActive(true);
			hud.ToggleHUDs(false);
			player.enabled = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			generator.config.seed = seed.text.Length > 0 ? Convert.ToInt32(seed.text) : 0;
			hud.SetSeed(generator.config.seed);
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
			hud.gameObject.transform.Find("HUDs/Title HUD/Resume/Continue").gameObject.GetComponent<Selectable>().Select();
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
			Time.timeScale = 0;
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Restart").gameObject.SetActive(true);
			state = State.END;
			hud.gameObject.transform.Find("HUDs/Title HUD/Restart/Restart").gameObject.GetComponent<Selectable>().Select();
		}
	}

	public void Win() {
		if(state != State.WIN) {
			hud.ToggleHUDs(false);
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			hud.SetSubtitle("You survive the night", Mathf.Infinity);
			Time.timeScale = 1;
			state = State.WIN;
		}
	}

	public void WinPause() {
		if(state != State.WINPAUSE) {
			hud.ToggleHUDs(true);
			hud._hurtTime = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 0;
			transform.Find("HUDs/Title HUD/Menu/Resume").gameObject.SetActive(false);
			transform.Find("HUDs/Title HUD/Menu/Restart").gameObject.SetActive(true);
			state = State.WINPAUSE;
			hud.gameObject.transform.Find("HUDs/Title HUD/Restart/Restart").gameObject.GetComponent<Selectable>().Select();
		}
	}
}
