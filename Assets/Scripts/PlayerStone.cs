using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour {


	// Use this for initialization
	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager> ();
		targetPosition = this.transform.position;
	}
		
	bool isAnimating = false;
	bool scoreMe = false;
	StateManager stateManager;
	Tile currentTile;
	Tile[] moveQueue;
	int moveQueueIndex = 9999;
	Vector3 targetPosition;
	Vector3 velocity = Vector3.zero;
	float smoothTime = 0.25f;
	float smoothTimeVertical = 0.1f;
	float smoothDistance = 0.1f;
	float smoothHeight = 0.5f;
	PlayerStone stoneToBop;

	public StoneStorage stoneStorage;
	public Tile startingTile;
	public int playerId;

	void Update(){
		if (isAnimating == false) 
		{
			return;
		}

		if (Vector3.Distance(
			new Vector3 (this.transform.position.x, targetPosition.y, this.transform.position.z),
			targetPosition)	< smoothDistance) 
		{
			// We've reached the target position -- do we still have moves in the queue?
			if ((moveQueue == null || moveQueueIndex == moveQueue.Length)
				&& 
				(this.transform.position.y-smoothDistance) > targetPosition.y
			) {
				// We are above target position
				// Out of moves, drop down the stone
				this.transform.position = Vector3.SmoothDamp (
					this.transform.position, 
					new Vector3 (this.transform.position.x, targetPosition.y, this.transform.position.z),
					ref velocity, 
					smoothTimeVertical);			

				// Check for bops
				if (stoneToBop != null) {
					stoneToBop.ReturnToStorage ();
					stoneToBop = null;
				}	
			}
			else
			{
				// Right position, right height, advance cube
				AdvanceMoveQueue();
			}
		}

		// Rise stone up before moving sideways
		else if (this.transform.position.y < (smoothHeight - smoothDistance)) 
			{
			this.transform.position = Vector3.SmoothDamp (
				this.transform.position, 
				new Vector3(this.transform.position.x, smoothHeight, this.transform.position.z),
				ref velocity, 
				smoothTimeVertical);			
		} 
		else 
		{
		this.transform.position = Vector3.SmoothDamp (
			this.transform.position, 
			new Vector3(targetPosition.x, smoothHeight, targetPosition.z), 
			ref velocity, 
			smoothTime);			
			}
		}

	void AdvanceMoveQueue()
	{
		if (moveQueue != null && moveQueueIndex < moveQueue.Length) {
			Tile nextTile = moveQueue [moveQueueIndex];
			if (nextTile == null) {
				// We are being scored
				// TODO: move to scored pile
				SetNewTargetPosition (this.transform.position + Vector3.right * 100f);
			} else {
				SetNewTargetPosition (nextTile.transform.position);
				moveQueueIndex++;
			}
		} 
		else 
		{
			// Movement queue is empty, so done animating
			stateManager.IsDoneAnimating = true;
			this.isAnimating = false;

			// Are we on roll again space
			if (currentTile != null && currentTile.isRollAgain) {
				stateManager.RollAgain ();
			}
		}
	}

	void SetNewTargetPosition (Vector3 pos)
	{
		targetPosition = pos;
		velocity = Vector3.zero;
		isAnimating = true;
	}

	void OnMouseUp(){
		// TODO: Check if mouse is over a UI element, ignore command if true

		// Is this correct player?
		if (stateManager.CurrentPlayerId != playerId) {
			return;
		}

		// Have we rolled the dice
		if (stateManager.IsDoneRolling == false) 
		{
			// We can't move yet
			return;
		}

		if (stateManager.IsDoneClicking == true) 
		{
			// We already done a move
			return;
		}
			
		int spacesToMove = stateManager.DiceTotal;

		moveQueue = GetTilesAhead (spacesToMove);
		Tile finalTile = moveQueue [moveQueue.Length - 1];

		if (finalTile == null) {
			// Scoring stone
			scoreMe = true;
		} else {
			if (CanLegallyMoveTo (finalTile) == false) {
				// Not allowed
				finalTile = currentTile;
				moveQueue = null;
				// stateManager.IsDoneClicking = false;
				return;
			}

			// If there is enemy tile in legal space, kick it out
			if (finalTile.PlayerStone != null) {
				stoneToBop = finalTile.PlayerStone;
				stoneToBop.currentTile.PlayerStone = null;
				stoneToBop.currentTile = null;
				// finalTile.PlayerStone.ReturnToStorage ();
			}
		}

		this.transform.SetParent (null); // Become Batman 

		// Remove ourselves	from our old tile
		if (currentTile != null) {
			currentTile.PlayerStone = null;
		}

		// Put ourselves in our new tile
		finalTile.PlayerStone = this;

		moveQueueIndex = 0;
		currentTile = finalTile;
		stateManager.IsDoneClicking = true;
		this.isAnimating = true;
	}

	public bool CanLegallyMoveAhead(int spacesToMove)
	{
		Tile theTile = GetTileAhead (spacesToMove);

		return CanLegallyMoveTo (theTile);
	}

	// Return list of tiles ___ moves ahead
	Tile[] GetTilesAhead(int spacesToMove)
	{
		if (spacesToMove == 0) {
			return null;
		}

		// Where should we end up?
		Tile finalTile = currentTile;
		Tile[] listOfTiles = new Tile[spacesToMove];

		for (int i = 0; i < spacesToMove; i++) {
			if (finalTile == null) {
				finalTile = startingTile;
			} else {
				if (finalTile.nextTiles == null || finalTile.nextTiles.Length == 0) {
					// We are overshooting victory pile -- just return some nulls in array
					// Break and return array with
					// nulls at the end
					break;
				} else if (finalTile.nextTiles.Length > 1) {
					// Branch based on player id
					finalTile = finalTile.nextTiles [playerId];
				} else {
					finalTile = finalTile.nextTiles [0];
				}
			}
			listOfTiles [i] = finalTile;
		}

		return listOfTiles;
	}

	// Return final tile we'd land if we moved __ spaces
	Tile GetTileAhead (int spacesToMove)
	{
		Tile[] tiles = GetTilesAhead (spacesToMove);

		if (tiles == null) {
			// We aren't moving at all, just return current tile
			return currentTile;
		}
		return tiles [tiles.Length - 1];	
	}

	bool CanLegallyMoveTo(Tile destinationTile)
	{
		Debug.Log ("Can LegallyMoveTo: " + destinationTile);

		if (destinationTile == null) {
			// Note! null tile means overshooting victory tile
			// Not legal move

			return false;
		}
		
		// Does tile have stone
		if (destinationTile.PlayerStone == null) {
			return true;
		}

		// Is it ally stone
		if (destinationTile.PlayerStone.playerId == this.playerId) {
			// Can't land on own stone
			return false;
		}

		// Is it enemy stone on a safe square
		if (destinationTile.isRollAgain == true) {
			return false;
		}

		// Can legally land on enemy stone and kick it off
		return true;
	}

	public void	ReturnToStorage()
	{
		//currentTile.PlayerStone = null;
		//currentTile = null;

		moveQueue = null;

		// Save current position
		Vector3 savePosition = this.transform.position;

		stoneStorage.AddStoneToStorage (this.gameObject);

		// Set new position to animation target
		SetNewTargetPosition(this.transform.position);

		// Restore saved position
		this.transform.position = savePosition;
	}
}
