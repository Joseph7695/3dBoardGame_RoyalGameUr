using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStorage : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// Create one stone for each placeholder spot

		for (int i = 0; i < this.transform.childCount; i++) {
			// Instantiate new copy of prefab
			GameObject theStone = Instantiate (StonePrefab);
			theStone.GetComponent<PlayerStone> ().startingTile = startingTile;
			theStone.GetComponent<PlayerStone> ().stoneStorage = this;

			AddStoneToStorage(theStone, this.transform.GetChild(i));
		}
	}

	public GameObject StonePrefab;
	public Tile startingTile;

	public void AddStoneToStorage (GameObject theStone, Transform placeholder=null)
	{
		if (placeholder == null) {
			// Find first empty placeholder
			for (int i = 0; i < this.transform.childCount; i++) {
				Transform p = this.transform.GetChild (i);
				if (p.childCount == 0) {
					// This placeholder is empty
					placeholder = p;
					break; // Break out of loop
				}
			}

			if (placeholder == null) {
				Debug.LogError ("Adding stone without empty spaces");
				return;
			}
		}

		// Parent stone to placeholder
		theStone.transform.SetParent(placeholder);

		// Reset stone local position in parent
		theStone.transform.localPosition = Vector3.zero;
	}
}
