using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		currentPlayerText = GetComponent<Text> ();
		stateManager = GameObject.FindObjectOfType<StateManager> ();
	}

	StateManager stateManager;
	Text currentPlayerText;

	// TODO: Consider humanizer library
	string[] numberWords = {"White", "Black"};

	// Update is called once per frame
	void Update () {

		currentPlayerText.text = "Current Player: Player " + numberWords [stateManager.CurrentPlayerId];
	}
}
