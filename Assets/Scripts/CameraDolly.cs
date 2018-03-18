using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour {

	// Use this for initialization
	void Start () {
		stateManager = GameObject.FindObjectOfType<StateManager> ();
	}

	StateManager stateManager;
	public float PivotAngle = 35f;
	float pivotVelocity;

	// Update is called once per frame
	void Update () {

		float angle = this.transform.rotation.eulerAngles.y;
		if (angle > 180) {
			angle -= 360;
		}

		angle = Mathf.SmoothDamp(
			angle, 
			(this.stateManager.CurrentPlayerId == 0 ? PivotAngle : -PivotAngle), 
			ref pivotVelocity, 
			0.25f);


		this.transform.rotation = Quaternion.Euler (new Vector3 (0, angle, 0));
	}
}
