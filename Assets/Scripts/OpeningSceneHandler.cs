using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSceneHandler : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (Input.anyKeyDown || Input.GetButton("Cross") || Input.GetButton("Square") || Input.GetButton("Triangle") || Input.GetButton("Circle") || Input.GetMouseButtonDown(0))
        {
			SceneManager.LoadScene("SailingScene");
        }
	}
}
