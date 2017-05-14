using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public float minimum;
	public float threshold;
	public float speed;
	public float damage;

	private GameObject _player;
	private Rigidbody _rb;

	public void Start() {
		_player = FindObjectOfType<GameController>().player.gameObject;
		_rb = GetComponent<Rigidbody>();
	}

	public void Update() {
		float distance = Vector3.Distance(transform.position, _player.transform.position);

		if(distance < threshold) {
			if(distance > minimum) {
				transform.LookAt(_player.transform);
				_rb.AddForce(transform.forward * speed);
			} else {
				// Attack player!
			}
		}
	}
}
