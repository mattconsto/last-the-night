using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	public GameObject parent = null;

	public GameObject decalPrefab = null;

	public float lifetime = 5; // Seconds

	public float bulletDamage = 1; // Damage per bullet
	public float damageFalloff = 0; // damage = distance^falloff, so 0=constant, 1=linear, 2=quadratic
	public float damageMinimum = 0; // Minimum damage after falloff
	public float damageSpread = 0; // How far the bullet spreads

	public float explosionDamage = 0; // Damage inside
	public float explosionFalloff = 0; // damage = distance^falloff, so 0=constant, 1=linear, 2=quadratic
	public float explosionRadius = 0; // How wide to look
	public bool  explosionFused = false; // pills/rockets

	public float knockbackForce = 0;

	public float criticalChance = 0; // Chance to deal criticals
	public float criticalMultiplier = 2; // Critical Multiplier

	public float incindiaryTime = 0; // Time player ignited for

	private float _lifetime;

	void Start() {
		_lifetime = lifetime;
	}

	void Update() {
		_lifetime -= Time.deltaTime;

		if(_lifetime < 0) {
			// Explode when they despawn.
			if(explosionFused) {

				GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
				foreach(GameObject player in targets) {
					float distance = (transform.position - player.transform.position).magnitude;
					if(distance <= explosionRadius) {
						float edamage = Mathf.Pow((explosionRadius - distance) / explosionRadius, explosionFalloff) * explosionDamage;
						player.GetComponent<EnemyController>().OnHurt(edamage, 0);
						player.GetComponent<Rigidbody>().AddForce(Vector3.up * knockbackForce);
					}
				}
			}

			Destroy(gameObject);
		}
	}

	void OnCollisionEnter(Collision col) {
		bool destroy = false;

		if(col.gameObject.tag == "Target") {
			print("Hit Target");
			destroy = true;
			float bdamage = Mathf.Max(damageMinimum, Mathf.Pow(_lifetime / lifetime, damageFalloff) + (Random.value - 0.5f) * damageSpread) * bulletDamage * (Random.value < criticalChance ? criticalMultiplier : 1);
			if(col.gameObject.GetComponent<Destructable>().health > 0) {
				col.gameObject.GetComponent<Destructable>().OnHurt(bdamage, incindiaryTime);
			}
		}

		if(explosionRadius > 0) {
			// Find enemies within radius

			GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
			foreach(GameObject player in targets) {
				float distance = (col.contacts[0].point - player.transform.position).magnitude;
				if(distance <= explosionRadius) {
					float edamage = Mathf.Pow((explosionRadius - distance) / explosionRadius, explosionFalloff) * explosionDamage;
					player.GetComponent<Destructable>().OnHurt(edamage, 0);
					player.GetComponent<Rigidbody>().AddForce(Vector3.up * knockbackForce);
				}
			}
		}

		if(destroy || col.gameObject.tag != "Unjumpable" && !explosionFused) {
			if(decalPrefab != null) {
				Instantiate(decalPrefab, col.contacts[0].point, Quaternion.FromToRotation(Vector3.forward, col.contacts[0].normal));
			}
			Destroy(gameObject);
		}
	}
}
