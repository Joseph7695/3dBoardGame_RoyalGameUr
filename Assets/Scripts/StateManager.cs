using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
		DiceTotalText.text = "= ???";
	}

	public Text DiceTotalText;
	public int DiceTotal;
	public int NumberOfPlayers = 2;
	public int CurrentPlayerId = 0;

	public bool IsDoneRolling = false;
	public bool IsDoneClicking = false;
	public bool IsDoneAnimating = false;

	public GameObject NoLegalMovesPopup;

	public void NewTurn()
	{
		// This is start of a player's turn
		// We don't have roll yet
		// doneRolling = false;
		IsDoneRolling = false;
		IsDoneClicking = false;
		IsDoneAnimating = false;

		CurrentPlayerId = (CurrentPlayerId + 1) % NumberOfPlayers;

		DiceTotalText.text = "= ???";
	}

	public void RollAgain()
	{
		IsDoneRolling = false;
		IsDoneClicking = false;
		IsDoneAnimating = false;
		DiceTotalText.text = "= ???";
	}

	// Enum example, (Enum is better and less likely for bugs)
	// public enum TurnPhase { WAITING_FOR_ROLL, WAITING_FOR_CLICK, WAITING_FOR_ANIMATION}
	// public TurnPhase CurrentPhase;
	// public void AdvancePhase()

	// Update is called once per frame
	void Update () {

		// Is turn done?
		if (IsDoneRolling && IsDoneClicking && IsDoneAnimating) {
			Debug.Log ("Turn is done");
			NewTurn ();

		}
	}

	public void CheckLegalMoves()
	{
		// If we roll zero, clearly no legal moves
		if (DiceTotal == 0) {
			StartCoroutine (NoLegalMoveCoroutine());
			return;
		}
		
		// Loop through all player stones
		PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
		bool hasLegalMove = false;
		foreach (PlayerStone ps in pss) {
			if (ps.playerId == CurrentPlayerId) {
				if (ps.CanLegallyMoveAhead(DiceTotal))
					{
						// TODO: Highlight stones that can be legally moved
						hasLegalMove = true;
					}
				}
			}
			
		// Move to next player if no moves available (give message)
		if (hasLegalMove == false) {
			StartCoroutine (NoLegalMoveCoroutine());
			return;
		}
	}

	IEnumerator NoLegalMoveCoroutine()
	{
		// Display message
		NoLegalMovesPopup.SetActive(true);

		// Wiat a second
		yield return new WaitForSeconds(1f);

		// Hide message
		NoLegalMovesPopup.SetActive(false);

		NewTurn();
	}
}
