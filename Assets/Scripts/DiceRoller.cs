using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {
	
	public int[] DiceValues;
	// Change to ? at player turn start;	

	public Sprite[] DiceImageOne;
	public Sprite[] DiceImageZero;

	StateManager stateManager;

	void Start()
	{
		stateManager = GameObject.FindObjectOfType<StateManager> ();
		DiceValues = new int[4];
	}

	public void RollTheDice()
	{
		// In Ur, you roll four dice (Clasically tetrahedron), which
		// have half their faces have value of "1" and half have "0" value

		// We can roll physics enabled dice

		// Going to use random number generation in this sample

		if (stateManager.IsDoneRolling == true) 
		{
			// Already rolled
			return;
		}

		stateManager.DiceTotal = 0;
		for (int i = 0; i < DiceValues.Length; i++) {
			DiceValues [i] = Random.Range (0,2);
			stateManager.DiceTotal += DiceValues [i];

			// Update visuals to show dice roll
			// Can include animation 2D / 3D

			// If have animation, wait for it before set doneRolling

			// We have 4 children, each is image of die. Grab that child
			// Update it's Image component to correct sprit

			if (DiceValues [i] == 0) {
				this.transform.GetChild (i).GetComponent<Image> ().sprite = 
					DiceImageZero[Random.Range (0, DiceImageZero.Length)];
			}
			else
				this.transform.GetChild (i).GetComponent<Image> ().sprite = 
					DiceImageOne[Random.Range (0, DiceImageOne.Length)];
		}
			
		stateManager.IsDoneRolling = true;
		//stateManager.DiceTotal = 20;
		stateManager.CheckLegalMoves ();

		stateManager.DiceTotalText.text = "= " + stateManager.DiceTotal;
		Debug.Log ("Rolled" + stateManager.DiceTotal);
	}
}
