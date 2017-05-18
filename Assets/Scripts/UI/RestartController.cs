using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartController : MonoBehaviour, IPointerClickHandler, ISubmitHandler {
	public void OnPointerClick(PointerEventData ed) {
		// Restart
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnSubmit(BaseEventData ed) {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
