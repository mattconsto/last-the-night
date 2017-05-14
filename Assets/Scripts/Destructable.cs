using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {
	public float health = 100;
	public float burnTime = 0f;

	private float _fireTimer = 0f;
	private float _bloodTimer = 0f;

	//private GameObject _fireParticles;
	//private GameObject _bloodParticles;

	public void Start() {
		//_fireParticles = transform.Find("Fire Particles").gameObject;
		//_bloodParticles = transform.Find("Blood Particles").gameObject;
	}

	public void Update() {
		if(_fireTimer > 0) {
			_fireTimer -= Time.deltaTime;
			OnHurt(Time.deltaTime * 5, 0);

			//var emission = _fireParticles.GetComponent<ParticleSystem>().emission;
			//emission.rateOverTime = 10;
		} else {
			//var emission = _fireParticles.GetComponent<ParticleSystem>().emission;
			//emission.rateOverTime = 0;
		}

		if(_bloodTimer > 0) {
			_bloodTimer -= Time.deltaTime;
		} else {
			//var emission = _bloodParticles.GetComponent<ParticleSystem>().emission;
			//emission.rateOverTime = 0;
		}
	}

	public void OnHurt(float damage, float fire) {
		_fireTimer = Mathf.Max(fire, _fireTimer);
		health -= damage;

		if(health <= 0) {
			gameObject.SetActive(false);
		} else {
			//var emission = _bloodParticles.GetComponent<ParticleSystem>().emission;
			//emission.rateOverTime = 10;
			_bloodTimer = 0.25f;
		}
	}
}
