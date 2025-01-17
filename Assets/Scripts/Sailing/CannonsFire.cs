using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//So the idea here is that we'll get the cannons firing...
public class CannonsFire : MonoBehaviour {
	public BoatSetup ourBoat;
	public GameObject CannonballPrefab;
	public GameObject BoatsLocation;
	//So our parent boat has cannon locations marked

	// Use this for initialization
	void Start () {
		if (!ourBoat)
		{
			ourBoat = gameObject.transform.parent.GetComponentInChildren<BoatSetup>();
		}
	}
	

	public void FireCannons(Vector3 StartDirection, float SpeedMultiplier)
    {
		if (!ourBoat)
		{
			ourBoat = gameObject.transform.parent.GetComponentInChildren<BoatSetup>();
		}
		//Essentially we've got to elect to fire a set of cannons here, and send the necessary command through to our individual spawn points
		for (int i = 0; i < ourBoat.NearCannons.Count; i++)
		{
			ourBoat.NearCannons[i].GetComponent<CannonBehavior>().FireCannon(ourBoat.gameObject, CannonballPrefab, StartDirection, BoatsLocation, 10f, SpeedMultiplier, 0.125f);
		}
	}
}
