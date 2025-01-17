using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBoatBehaviors
{
	public float moveRange = 38f;
	public float moveSpeed = 0.5f; //1 would mean we're following a sine wave constantly
	public float moveOffset = 0f; //just to add variance
	public float pushBack = 0; //Potentially for stacking boats, potentially for not
}

public class EnemyBoatPositionController : MonoBehaviour {
	public enum enEnemyFiringState { NULL, ACTIVE, FINISHED, BROKEN }
	public enEnemyFiringState EnemyFiringState = enEnemyFiringState.NULL;
	public GameObject BoatsLocation;    //For reference with projectiles
	public GameObject CannonballPrefab; //PROBLEM: I'm sure we'll want to mix this up, but for the moment

	public List<GameObject> EnemyBoats; //This will need to be populated depending on the challenge, but the idea is that these will stack
	public List<EnemyBoatBehaviors> EnemyBoatBehavior = new List<EnemyBoatBehaviors>();

	public GameObject LeftAimMarker, RightAimMarker;

	// Update is called once per frame
	void Update() {
		if (SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.FINISHED)
		{

		}
		else
		{
			//PROBLEM: This isn't smoothed when it comes to setting positions, and we'll need to implement something different to avoid jumps
			if (SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.PLAYER || SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.OPENING)
			{
				for (int i = 0; i < EnemyBoats.Count; i++)
				{
					if (EnemyBoats[i].GetComponent<EnemyBoatCombatBehavior>().bIsAlive())
					{
						//PROBLEM: We probably need a configurable offset for the spaces between boats
						EnemyBoats[i].transform.localPosition = new Vector3(-7f * i, 0, Mathf.Sin((Time.time + EnemyBoatBehavior[i].moveOffset) * EnemyBoatBehavior[i].moveSpeed) * EnemyBoatBehavior[i].moveRange);
						EnemyBoats[i].transform.localEulerAngles = Vector3.one;
					}
				}
			}
		}
	}

	//This gets called by the controller when we want to start our attack phase
	public void StartEnemyVolleys()
	{
		//need to start our helper that'll control which boats are going into firing positions. Lets just toss this onto an coroutine
		StartCoroutine(DoEnemyAttacks());
	}

	public void BreakEnemyVolleys()
	{
		KillCoroutines();
		StartCoroutine(BreakEnemyRoutine());
	}

	IEnumerator BreakEnemyRoutine()
    {
		yield return new WaitForSeconds(1f);
		SailingCanvasController.Instance.SetAndDisplayMessage("BREAKER!", 1f);
		//We'll need some sort of cleanup here pertaining to enemy shot markers
		for (int i = 0; i < EnemyBoats.Count; i++)
		{
			Enemy_BoatFiringMarker BracketMarkers = EnemyBoats[i].GetComponent<EnemyBoatCombatBehavior>().ourFiringMarker;
			BracketMarkers.gameObject.SetActive(false);
		}
		SailingGameController.Instance.CurrentCombatController.ReturnToPlayerControl();
		//Deselect our options
		SailingGameController.Instance.CurrentCombatController.SelectPlayerWeapon(SailingCombatController.enPlayerWeapon.NONE);
	}

	public void KillCoroutines()
    {
		StopAllCoroutines();
    }

	Vector3[] CalculateBrackets(int Bracket, BoatSetup TargetBoatSetup)
	{
		//So for our bracket we can take the corners of our boats bounding box I guess, or the cannons?
		Vector3 NearLeftCorner = Vector3.zero;
		Vector3 NearRightCorner = Vector3.zero;
		//I want to start with the easy systems here
		//In theory our cannons should always be ordered so near cannons are ordered front/back meaning
		Vector3 FarLeftCorner = TargetBoatSetup.NearCannons[TargetBoatSetup.NearCannons.Count - 1].transform.position;
		Vector3 FarRightCorner = TargetBoatSetup.NearCannons[0].transform.position;

		//For the moment
		Vector3[] BracketVector = new Vector3[4];
		#region FramingBrackets
		if (Bracket < 3)
		{
			switch(Bracket)
            {
				case 0:
					NearLeftCorner = LeftAimMarker.transform.position;
					NearRightCorner = RightAimMarker.transform.position;
					break;
				case 1:
					NearLeftCorner = LeftAimMarker.transform.position;
					NearRightCorner = Vector3.Lerp(LeftAimMarker.transform.position, RightAimMarker.transform.position, 0.6f);
					break;
				case 2:
					NearLeftCorner = Vector3.Lerp(LeftAimMarker.transform.position, RightAimMarker.transform.position, 0.4f);
					NearRightCorner = RightAimMarker.transform.position;
					break;
			}

			BracketVector = new Vector3[8];
			BracketVector[0] = (NearLeftCorner);
			BracketVector[1] = (NearRightCorner);
			BracketVector[2] = (FarLeftCorner);
			BracketVector[3] = (FarRightCorner);

			//Add in an empty point so that our arrays are always the same size and we don't get any engine complaints
			BracketVector[4] = NearLeftCorner;
			BracketVector[5] = NearLeftCorner;
			BracketVector[6] = NearLeftCorner;
			BracketVector[7] = NearLeftCorner;
		}
		else
		{
			//Special case happening!
			BracketVector = new Vector3[8];
			//Left hand bracket
			BracketVector[0] = (LeftAimMarker.transform.position);
			BracketVector[1] = (Vector3.Lerp(LeftAimMarker.transform.position, RightAimMarker.transform.position, 0.3f));
			BracketVector[2] = (FarLeftCorner);
			BracketVector[3] = (FarRightCorner);

			//Right Hand Bracket
			BracketVector[4] = (Vector3.Lerp(RightAimMarker.transform.position, LeftAimMarker.transform.position, 0.3f));
			BracketVector[5] = (RightAimMarker.transform.position);
			BracketVector[6] = (FarLeftCorner);
			BracketVector[7] = (FarRightCorner);
		}
		return BracketVector;
	}

	void CheckEnemyBoatList()
    {
		List<GameObject> BoatsToRemove = new List<GameObject>();
		foreach (GameObject ThisBoat in EnemyBoats)
        {
			BoatSetup EnemyBoat = ThisBoat.GetComponentInChildren<BoatSetup>();
			if (EnemyBoat.BaseHealth <= 0)
            {
				BoatsToRemove.Add(ThisBoat);
            }
        }
		if (BoatsToRemove.Count > 1)
        {
			Debug.LogError("Removing Boat Entry from List");
			foreach (GameObject BoatToRemove in BoatsToRemove)
            {
				EnemyBoats.Remove(BoatToRemove);
            }
        }
    }

	IEnumerator DoEnemyAttacks() { 
		EnemyFiringState = enEnemyFiringState.ACTIVE;

		CheckEnemyBoatList();	//Quick issue check pass to make sure we're not acting on anything that's in the act of dying

		//So lets just go through our boats in order
		for (int i=0; i<EnemyBoats.Count; i++)
        {

			BoatSetup TargetBoatSetup = EnemyBoats[i].GetComponentInChildren<BoatSetup>();

			float MoveStartTime = Time.time;
			float MoveTime = 1f;
			while (MoveStartTime + MoveTime > Time.time)
			{
				yield return null;
				//We need to reposition our boats. I need a clever function that'll shift the target boat to the center, and the others out of the way
				float LerpT = 0f;
				//PROBLEM: This isn't a good way to move the boats into position
				while (Time.time < (MoveStartTime + MoveTime))
				{
					LerpT = (Time.time - MoveStartTime) / MoveTime;

					for (int b = 0; b < EnemyBoats.Count; b++)
					{
						if (b == i) //This is our target boat, lets move it to the middle
						{
							EnemyBoats[b].transform.localPosition = Vector3.Lerp(EnemyBoats[i].transform.localPosition, Vector3.zero - Vector3.right * 7f * b, Time.deltaTime * 2f);
							//Our rotation can get screwy
							
						} else
                        {
							//seeing as we've only got 3 with the 3rd being in the back (like a frigate)
							if (b==0)
                            {
								EnemyBoats[b].transform.localPosition = Vector3.Lerp(EnemyBoats[b].transform.localPosition, Vector3.forward * 40f - Vector3.right * 7f * b, Time.deltaTime * 2f);
							} else if (b==1)
                            {
								EnemyBoats[b].transform.localPosition = Vector3.Lerp(EnemyBoats[b].transform.localPosition, -Vector3.forward * 40f - Vector3.right * 7f * b, Time.deltaTime * 2f);
							} else
                            {
								EnemyBoats[b].transform.localPosition = Vector3.Lerp(EnemyBoats[b].transform.localPosition, Vector3.zero - Vector3.right * 7f * b, Time.deltaTime * 2f);
							}							
						}
						EnemyBoats[b].transform.localEulerAngles = Vector3.Lerp(EnemyBoats[b].transform.localEulerAngles, Vector3.zero, Time.deltaTime * 2f);
					}

					yield return null;
				}
			}

			yield return new WaitForSeconds(1f); //pre pause after moving boats into position
			//So in theory we've moved into position, and can commence the firing sequence
			int NumVolleys = 3;

			List<Enemy_BoatFiringMarker> FiringMarkers = new List<Enemy_BoatFiringMarker>();
			for (int v = 0; v < NumVolleys; v++)
			{

				//Need something to decide what the marker is...0 center, 1 left, 2 right, 3 full (unlikely)
				//What we could do here is make a random float, take the top off for the full bracket, and then divide the remainer into 3
				//What we could also do is be lazy

				int Bracket = Mathf.FloorToInt(Random.Range(0, 3)) + 1;
				//Bracket = 0;
				if (Random.value > 0.9f) {  //do a full area shot
					Bracket = 0;				
				}
				//Debug.Log("Bracket Int: " + Bracket);
				Vector3[] BracketVector = CalculateBrackets(Bracket, TargetBoatSetup);

				//So our boat should have some bracket objects
				Enemy_BoatFiringMarker BracketMarkers = EnemyBoats[i].GetComponent<EnemyBoatCombatBehavior>().ourFiringMarker;//EnemyBoats[i].GetComponentInChildren<Enemy_BoatFiringMarker>();
				BracketMarkers.gameObject.SetActive(true);	//Turn our brackets on
				BracketMarkers.SetFiringZone(BracketVector);
				#endregion
				//Debug.Log(BracketVector[1]);
				//Debug.Log(NearRightCorner);
				//After the bracket markers have been drawn we need to animate the indicator line so that it does what we want it to
				#region StartAnimation
				float WarmInStart = Time.time;
				float WarmInTime = 0.5f;
				while (Time.time < WarmInStart + WarmInTime)
				{
					float WarmInOpacity = (Time.time - WarmInStart) / WarmInTime;
					//foreach (Enemy_BoatFiringMarker thisMarker in FiringMarkers)
					//{
					BracketMarkers.SetMarkerDetails(WarmInOpacity, 0.1f);
					//}
					yield return null;
				}
                #endregion
                #region LineAnimation
                float LineStartTime = Time.time;
				float LineSpeed = 0.75f;
				while (Time.time < LineStartTime + LineSpeed)
				{
					float LinePosition = Mathf.Lerp(0.1f, 1.0f, (Time.time - LineStartTime) / LineSpeed);
					//foreach (Enemy_BoatFiringMarker thisMarker in FiringMarkers)
					//{
					BracketMarkers.SetMarkerDetails(1f, LinePosition);
					//}
					yield return null;
				}
                #endregion
                #region Firing
                float FiringStartTime = Time.time;

				//fire a volley to match the spread of our marker
				//So our far cannons are right to left. In theory we should be able to simply pick the spread from the markerset above
				//Debug.LogError("FarCannons Count: " + TargetBoatSetup.FarCannons.Count);
				//Reframe our brackets so we're correct to the time:
				SailingGameController.Instance.CurrentCombatController.PlayerBoatSetup.SetDodgeState(true); //So our player controller can keep check of successful dodges

				BracketVector = CalculateBrackets(Bracket, TargetBoatSetup);
				for (int cannon = 0; cannon < TargetBoatSetup.FarCannons.Count; cannon++)
				{
					Vector3 StartDirection = Vector3.forward;
					Vector3 CannonballStart = Vector3.zero;
					Vector3 CannonballEnd = Vector3.one;
					if (Bracket < 3)
					{
						float cannonLerp = 1f - ((float)cannon / (float)(TargetBoatSetup.FarCannons.Count - 1));
						CannonballStart = TargetBoatSetup.FarCannons[cannon].transform.position;
						CannonballEnd = Vector3.Lerp(BracketVector[0], BracketVector[1], cannonLerp);   //This isn't working as expected for some reason, and it probably has to do with it not being in local space to the mover
					} 
					else
                    {
						//We've a slightly more complicated firing pattern here where the cannons fire in a split,
						//so the first half are split between BracketVectors 0-1 and the second 4-5
						CannonballStart = TargetBoatSetup.FarCannons[cannon].transform.position;
						if (cannon < (TargetBoatSetup.FarCannons.Count/2))
                        {
							float cannonLerp = 1f - ((float)cannon / (float)(TargetBoatSetup.FarCannons.Count / 2 - 1));
							CannonballEnd = Vector3.Lerp(BracketVector[0], BracketVector[1], cannonLerp);
						} else
                        {
							float cannonLerp = 1f - ((float)(cannon- Mathf.FloorToInt(TargetBoatSetup.FarCannons.Count / 2)) / (float)(TargetBoatSetup.FarCannons.Count / 2 - 1));
							CannonballEnd = Vector3.Lerp(BracketVector[4], BracketVector[5], cannonLerp);
						}
					}

					StartDirection = BoatsLocation.transform.InverseTransformDirection(Vector3.Normalize(CannonballEnd - CannonballStart));
					float SpeedMultiplier = 2.5f; //PROBLEM: Adjust this for difficulty
					TargetBoatSetup.FarCannons[cannon].GetComponent<CannonBehavior>().FireCannon(TargetBoatSetup.gameObject, CannonballPrefab, StartDirection, BoatsLocation, 10f, SpeedMultiplier, 0.125f);
				}
                #endregion
                #region FinishAnimations
                float WarmOutStart = Time.time;
				float WarmOutTime = 0.5f;
				while (Time.time < WarmOutStart + WarmOutTime)
				{
					float WarmOutOpacity = 1f-((Time.time - WarmOutStart)) / WarmOutTime;
					BracketMarkers.SetMarkerDetails(WarmOutOpacity, 0.1f);
					yield return null;
				}
				BracketMarkers.gameObject.SetActive(false);  //Turn our brackets off
				#endregion
				//So as to make this syncronised and give the player a chance lets put a syncronising pause here
				yield return new WaitForSeconds(2f - (Time.time - FiringStartTime));
				SailingGameController.Instance.CurrentCombatController.PlayerBoatSetup.SetDodgeState(false); //So our player controller can keep check of successful dodges
																											//Have shader for bracket (also useful for player?)
																											//Have bracket animate

				//Release the volley in a spread that matches the bracket
				//Check shots missed the player? Add to dodge counter
				//Wait for the predescribed time for the player boat to return to neutral
			}
		}

		//After this point we can cycle back to the players turn
		yield return new WaitForSeconds(1f);
		SailingGameController.Instance.CurrentCombatController.ReturnToPlayerControl();
	}
}
