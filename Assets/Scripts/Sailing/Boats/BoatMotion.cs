using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMotion : MonoBehaviour {

	public float RockSpeed = 1f;
	public float RockAmount = 10f; //In deg
	public float PitchSpeed = 0.75f;
	public float PitchAmount = 3f;
	public float LiftSpeed = 0.5f;
	public float LiftAmount = 1f;

	Vector3 StartPosition = Vector3.zero;
	Vector3 StartRotation = Vector3.zero;
	// Use this for initialization
	void Start () {
		StartPosition = transform.localPosition;
		StartRotation = transform.localEulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = StartPosition + Vector3.up*(Mathf.Sin(Time.time * LiftSpeed) * LiftAmount);
		transform.localEulerAngles = new Vector3(StartRotation.x + Mathf.Sin(Time.time * PitchSpeed) * PitchAmount,
			StartRotation.y,
			StartRotation.z + Mathf.Sin(Time.time * RockSpeed) * RockAmount);
	}
}
