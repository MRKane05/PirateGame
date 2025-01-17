using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSetup : MonoBehaviour {
	public GameObject NearCannonSet, FarCannonSet;	//We'll put our child positions for cannons under this
	[HideInInspector]
	public List<GameObject> NearCannons = new List<GameObject>(); //Which cannon points do we use if we're controlling the boat
	[HideInInspector]
	public List<GameObject> FarCannons = new List<GameObject>(); //Which cannon points do we use if we're the boat being shot at?
	public GameObject CameraTarget;
	public GameObject GunneryCamera;
	public bool bIsEnemy = false;

	public float BaseHealth = 20f;
	float StartingHealth = 20;
	public GameObject HealthBarLocation;    //Used when this is an enemy

	[HideInInspector] public GameObject AssignedHealthBar;
	[HideInInspector] public HealthBarBehavior HealthBar;

	public GameObject CannonPrefab;
	public int VirtualCannons = 4;
	List<GameObject> PopulateCannonSet(Transform Parent, GameObject CannonA, GameObject CannonB, int ExtraCount)
    {
		List<GameObject> CannonSet = new List<GameObject>();
		CannonSet.Add(CannonA.gameObject);
		for (int i = 0; i<ExtraCount; i++)
        {
			float CannonFrac = (float)i / (float)ExtraCount;
			Vector3 CannonPosition = Vector3.Lerp(CannonA.transform.position, CannonB.transform.position, CannonFrac);
			GameObject newCannon = Instantiate(CannonPrefab, CannonPosition, CannonA.transform.rotation, Parent);
			CannonSet.Add(newCannon);
        }
		CannonSet.Add(CannonB.gameObject);
		return CannonSet;
	}

	public void SetBoatDetails(float newHealth, int newVirtualCannons)
    {
		BaseHealth = newHealth;
		VirtualCannons = newVirtualCannons;
    }

	// Use this for initialization
	void Start () {
		if (NearCannonSet)
        {
			CannonBehavior[] CannonSpread = NearCannonSet.GetComponentsInChildren<CannonBehavior>();
			NearCannons = PopulateCannonSet(NearCannonSet.transform, CannonSpread[0].gameObject, CannonSpread[1].gameObject, VirtualCannons);
        }
		if (FarCannonSet)
        {
			CannonBehavior[] CannonSpread = FarCannonSet.GetComponentsInChildren<CannonBehavior>();
			FarCannons = PopulateCannonSet(FarCannonSet.transform, CannonSpread[0].gameObject, CannonSpread[1].gameObject, VirtualCannons);
		}

		StartingHealth = BaseHealth;
	}

	//This'll only be for enemies
	public void SetHealthBar(GameObject newHealthBar)
    {
		AssignedHealthBar = newHealthBar;
		HealthBar = AssignedHealthBar.GetComponent<HealthBarBehavior>();
    }

    #region PlayerCombatCode
    int DodgeCount = 0;
	bool bDodgeState = false;
	public void SetDodgeState(bool newDodgeState)
    {
		if (!bDodgeState && newDodgeState)	//turning dodge state on
        {
			bDodgeState = true;
        }
		else if (bDodgeState && !newDodgeState) //turning dodge state off
        {
			bDodgeState = false;
			DodgeCount++;
			SailingCanvasController.Instance.SetAndDisplayMessage("Dodge Count: " + DodgeCount.ToString(), 1f);

			//So we can probably look at finding the breaker on the same level as ourselves, and driving it's state from there
			Weapon_Breaker breakerBase = transform.parent.gameObject.GetComponentInChildren<Weapon_Breaker>();
			if (breakerBase)
            {
				//Debug.Log("Found Breaker Button");
				if (breakerBase.AssignedSelectButton)
                {
					//Debug.Log("DodgeFraction: ");
					breakerBase.AssignedSelectButton.SetFillAmount(Mathf.Clamp01((float)DodgeCount / 4f));
				}
            }
		}
    }
    #endregion

    public virtual void TakeDamage(float DamageAmount)
    {
		//Debug.LogError("Boat Took Damage: " + DamageAmount);
		BaseHealth -= DamageAmount;
		if (BaseHealth < 0)
        {
			Debug.Log("Boat Destroyed: " + gameObject.name);
			Collider[] AllColliders = gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider thisCollider in AllColliders)
            {
				thisCollider.enabled = false; //Turn the collider off so that the boat can sink
            }
        }
		if (AssignedHealthBar)
        {
			Debug.Log("Setting Health");
			HealthBar.setBarHealth(Mathf.Clamp01(BaseHealth / StartingHealth));
		}
		if (bDodgeState)
        {
			bDodgeState = false;
			DodgeCount = 0;
        }
    }

	public void UsedBreaker()
    {
		DodgeCount = 0;
		Weapon_Breaker breakerBase = transform.parent.gameObject.GetComponentInChildren<Weapon_Breaker>();
		if (breakerBase)
		{
			//Debug.Log("Found Breaker Button");
			if (breakerBase.AssignedSelectButton)
			{
				//Debug.Log("DodgeFraction: ");
				breakerBase.AssignedSelectButton.SetFillAmount(Mathf.Clamp01((float)DodgeCount / 2f));
			}
		}
	}

	//This is intended for a clever functionality. It's not what it got
	public void DoBoatSink()
    {

    }
}
