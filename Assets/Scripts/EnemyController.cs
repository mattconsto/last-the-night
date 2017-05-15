using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public float minimum;
	public float threshold;
	public float speed = 1;
	public float damage = 0;
	public float fireDamage = 0;
	public float fireRate = 1;
	public float angle = 45;
	public bool angel = false;

	private float _cooldown = 0;

	private GameController _controller;
	private GameObject _player;
	private Rigidbody _rb;

	public void Start() {
		_controller = FindObjectOfType<GameController>();
		_player = _controller.player.gameObject;
		_rb = GetComponent<Rigidbody>();
	}

	public void Update() {
		// Stop attacking when game is over
		if(_controller.state == GameController.State.WIN) enabled = false;

		_cooldown -= Time.deltaTime;

		float distance = Vector3.Distance(transform.position, _player.transform.position);

		if(distance < threshold) {
			if(distance > minimum) {
				if(!angel || Vector3.Angle(_player.transform.forward, transform.position - _player.transform.position) > angle) {
					transform.LookAt(_player.transform);
					_rb.AddForce(transform.forward * speed);
				}
			} else {
				if(_cooldown < 0) {
					_player.GetComponent<Destructable>().OnHurt(damage, fireDamage);
					_cooldown = fireRate;
				}
			}
		}
	}
}
