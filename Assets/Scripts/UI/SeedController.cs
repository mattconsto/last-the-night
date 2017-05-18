using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedController : MonoBehaviour {
	public void Start() {
		// Pick a random seed
		int seed = new System.Random().Next(0, 10000000);
		Debug.Log("Seed: " + seed);
		GetComponent<InputField>().text = "" + seed;
	}
}
