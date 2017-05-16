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
	public bool attacking = false;

	private float _cooldown = 0;

	public AudioClip attackClip;
	private AudioSource source;

	private GameController _controller;
	private TimeController _time;
	private GameObject _player;
	private Rigidbody _rb;

	public void Start() {
		_controller = FindObjectOfType<GameController>();
		_time = FindObjectOfType<TimeController>();
		_player = _controller.player.gameObject;
		_rb = GetComponent<Rigidbody>();
		source = _controller.transform.Find("Sounds/Effects").GetComponent<AudioSource>();
	}

	public void Update() {
		// Stop attacking when game is over
		if(_controller.state == GameController.State.WIN) enabled = false;

		_cooldown -= Time.deltaTime;

		float distance = Vector3.Distance(transform.position, _player.transform.position);

		if(distance < threshold) {
			if(distance > minimum && attacking == false) {
				if(!angel || Vector3.Angle(_player.transform.forward, transform.position - _player.transform.position) > angle) {
					Debug.Log("Moving");
					transform.LookAt(_player.transform);
					_rb.AddForce(transform.forward * speed * (1 + _time.time * (1 + _controller.difficulty)));
				}
			} else {
				attacking = true;
				if(_cooldown < 0) {
					Debug.Log("Attacking");
					_player.GetComponent<Destructable>().OnHurt(damage * (1 + _time.time * (1 + _controller.difficulty)), fireDamage);
					_cooldown = fireRate;
					source.PlayOneShot(attackClip, 1);
				}
			}
		}
		attacking = false;
	}
}
