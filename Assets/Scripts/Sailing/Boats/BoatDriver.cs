using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is just a rough as guts setup class at this stag
public class BoatDriver : MonoBehaviour {
	Rigidbody ourRigidBody;
	public float[] travelForces;
	public int forwardFactor = 0;

	public float rotationalForce = 10f;

	// Use this for initialization
	void Start () {
		ourRigidBody = gameObject.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update() {
		if (SailingGameController.Instance)
        {
			if (SailingGameController.Instance.GameplayMode == SailingGameController.enGameMode.SAILING)
            {
				DriveBoat();
            }
        }
	}

	void DriveBoat() { 
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			forwardFactor++;
			forwardFactor = Mathf.Clamp(forwardFactor, 0, 3);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			forwardFactor--;
			forwardFactor = Mathf.Clamp(forwardFactor, 0, 3);
		}

		ourRigidBody.AddForce(transform.forward*travelForces[forwardFactor], ForceMode.Acceleration);

		if (Input.GetKey(KeyCode.RightArrow))
		{
			ourRigidBody.AddTorque(new Vector3(0, rotationalForce, 0), ForceMode.Acceleration);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			ourRigidBody.AddTorque(new Vector3(0, -rotationalForce, 0), ForceMode.Acceleration);
		}
	}
}
