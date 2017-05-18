using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {
	public float health = 1;
	public float maxHealth = 1;
	public float stamina = 1;
	public float maxStamina = 1;
	public float regenRate = 0.1f;
	public float decayRate = 0.1f;

	public float _healthTimer = 0f; // Don't instantly regen
	public float _staminaTimer = 0f;
	private float _fireTimer = 0f; // Time we burn

	public void Update() {
		_healthTimer -= Time.deltaTime;
		_staminaTimer -= Time.deltaTime;

		if(_healthTimer < 0 && health < maxHealth + decayRate) {
			health += regenRate * Time.deltaTime;
		} else if(health > maxHealth + decayRate) {
			health -= decayRate * Time.deltaTime;
		}

		if(_staminaTimer < 0 && stamina < maxStamina + decayRate) {
			stamina += regenRate * Time.deltaTime;
		} else if(stamina > maxStamina + decayRate) {
			stamina -= decayRate * Time.deltaTime;
		}

		if(_fireTimer > 0) {
			_fireTimer -= Time.deltaTime;
			OnHurt(Time.deltaTime * 5, 0);
		}
	}

	public void OnHurt(float damage, float fire) {
		_fireTimer = Mathf.Max(fire, _fireTimer);
		health -= damage;
		_healthTimer = 2f;

		if(health <= 0) {
			gameObject.SetActive(false);
		}
	}
}
