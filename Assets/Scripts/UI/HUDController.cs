using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
	public GameObject titleHUD;
	public GameObject gameHUD;
	public Text subtitlesText;
	public Text hintText;
	public RawImage hurtImage;
	public GameObject healthBar;
	public GameObject staminaBar;

	private float _subtitletime = 0;
	private float _hurtTime = 0;
	private float _health = 1;
	private float _stamina = 1;
	private float _healthScale = 0;
	private float _staminaScale = 0;
	private Text _healthText;
	private Text _staminaText;

	public void Start() {
		// Grab values and components for later
		_healthScale  = healthBar.transform.localScale.x;
		_staminaScale = staminaBar.transform.localScale.x;
		_healthText   = healthBar.GetComponent<Text>();
		_staminaText  = staminaBar.GetComponent<Text>();
	}

	public void Update() {
		_subtitletime -= Time.deltaTime;
		_hurtTime     -= Time.deltaTime;

		// Remove subtitles when their time runs out
		if(_subtitletime < 0) subtitlesText.text = "";

		// Fade out the hurt vingette
		hurtImage.SetAlpha(Mathf.Clamp01(_hurtTime));
	}

	public void SetSubtitle(string text, float time) {
		subtitlesText.text = text;
		_subtitletime = time;
	}

	public void SetHint(string text) {
		hintText.text = text;
	}

	public void SetHealth(float value) {
		float temp = Mathf.Clamp01(value);
		if(temp < _health) _hurtTime = 2;
		_health = temp;

		// Change the opacity and width as health decreases
		_healthText.SetAlpha(Mathf.Clamp01(_health * 10));
		healthBar.transform.SetLocalScaleX(_health * _healthScale);
	}

	public void SetStamina(float value) {
		_stamina = Mathf.Clamp01(value);

		// Change the opacity and width as stamina decreases
		_staminaText.SetAlpha(Mathf.Clamp01(_stamina * 10));
		staminaBar.transform.SetLocalScaleX(_stamina * _staminaScale);
	}

	public void ToggleHUDs() {
		titleHUD.SetActive(!titleHUD.activeSelf);
		gameHUD.SetActive(!gameHUD.activeSelf);
	}
}
