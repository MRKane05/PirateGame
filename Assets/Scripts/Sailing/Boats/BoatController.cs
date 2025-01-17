using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A basic "driver" for the boat object that'll control it like a car
public class BoatController : MonoBehaviour {
	public float BoatSpeed = 0;	//How fast are we moving?
	public float Accelleration = 1f;
	public float Deceleration = 1f;	//This might be a pipe dream

	public float CurrentSpeed = 0;
	public float[] Speeds;

	public float TurningSpeed = 0;  //How fast are we turning?
	public float TurnAccelleration = 1f;
	public float MaxTurnSpeed = 30f; //How fast can we turn?

	Vector3 CollisionBounce = Vector3.zero;
	Vector3 transformMove = Vector3.zero;   //How much have we moved this frame?

	// Update is called once per frame
	void Update()
	{
		if (SailingGameController.Instance)
		{
			if (SailingGameController.Instance.GameplayMode == SailingGameController.enGameMode.SAILING)
			{
				DriveBoat();
			}
		} 
	}

	void DriveBoat() {
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			CurrentSpeed++;
			CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0, 3);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			CurrentSpeed--;
			CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0, 3);
		}
#endif
		CurrentSpeed = Mathf.Clamp(CurrentSpeed - Input.GetAxis("Left Stick Vertical") * Time.deltaTime, 0, 3);
		//Handle our target speeds somehow (slerp?)
		BoatSpeed = Mathf.Lerp(BoatSpeed, Speeds[Mathf.RoundToInt(CurrentSpeed)], Time.deltaTime * Accelleration);

		float TurningTarget = 0;
#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.RightArrow))
		{
			TurningTarget = MaxTurnSpeed;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			TurningTarget = -MaxTurnSpeed;
		}
#endif
#if !UNITY_EDITOR
		TurningTarget = Input.GetAxis("Left Stick Horizontal")* MaxTurnSpeed;
#endif

		TurningSpeed = Mathf.Lerp(TurningSpeed, TurningTarget, Time.deltaTime * TurnAccelleration);
		transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y + TurningSpeed * Time.deltaTime, 0f); //So that collisions won't change our orientation

		CollisionBounce = Vector3.Lerp(CollisionBounce, Vector3.zero, Time.deltaTime * Accelleration);
		//Now move us
		transformMove = (transform.forward * BoatSpeed + CollisionBounce) * Time.deltaTime;
		transform.position = new Vector3(transform.position.x + transformMove.x, 0f, transform.position.z + transformMove.z);	//So we can never leave the water
	}

	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Got Collision With: " + collision.gameObject.name);
		//Hopefully never more than one contact point...
		Vector3 collisionVector = Vector3.zero;
		foreach (ContactPoint contact in collision.contacts)
		{
			//Debug.DrawRay(contact.point, contact.normal, Color.white);
			collisionVector += (contact.point - transform.position);
		}
		collisionVector /= collision.contacts.Length;   //Get an average I guess
		//Shunt us somewhere safe
		gameObject.transform.position = new Vector3(gameObject.transform.position.x - collisionVector.x*0.5f, 0, gameObject.transform.position.z - collisionVector.z*0.5f); //Move us out of the collision

		Debug.Log("Collision Dot: " + Vector3.Dot(collisionVector.normalized, transformMove.normalized));   //So if this is > 0 our collision was in front of us or off to the side

		BoatSpeed = 0;
		CurrentSpeed = 0;
		CollisionBounce = new Vector3(-collisionVector.x, 0, -collisionVector.z);//This'll need scaled
		//CollisionBounce *= 3f;
		//In theory if this is terrain we can just bounce backwards

		/*
		//For playing collision sounds
		if (collision.relativeVelocity.magnitude > 2)
			audioSource.Play();
		*/
	}
}
