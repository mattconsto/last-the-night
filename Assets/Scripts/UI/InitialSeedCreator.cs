using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialSeedCreator : MonoBehaviour {
	public void Start () {
		GetComponent<InputField>().text = "" + new System.Random().Next(0, 10000000);
	}
}
