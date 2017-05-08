using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartController : MonoBehaviour, IPointerClickHandler {
	public void OnPointerClick(PointerEventData ed) {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
