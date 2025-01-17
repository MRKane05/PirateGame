using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//At the moment this script is soely responsible for keeping an eye on the countdown timer for the boat to see if it's ready to shoot
public class EnemyBoatCombatBehavior : MonoBehaviour {
	float ReloadTime = 15f;
	float CooldownRemaining;
	public bool bIsReloaded = false;
	public bool bIsAlive() {
		if (ourBoat) { return ourBoat.BaseHealth > 0; } else return false; } 
	BoatSetup ourBoat;
	public Enemy_BoatFiringMarker ourFiringMarker;

	void Start()
    {
		ourBoat = gameObject.GetComponent<BoatSetup>();
		CooldownRemaining = 3f; //Give us a quick look before the state changes
		ourBoat = gameObject.GetComponentInChildren<BoatSetup>();
    }

	public void ResetCountdown()
    {
		//Debug.Log("Resetting Countdown");
		CooldownRemaining = ReloadTime;
		bIsReloaded = false;
	}

	// Update is called once per frame
	void Update () {
		if (bIsAlive())
		{
			if (SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.PLAYER)
			{
				CooldownRemaining -= Time.deltaTime;
			}
			bIsReloaded = CooldownRemaining <= 0f;  //Set this to true so that the system can keep an eye on things

			//We need to set our cooldown icon
			ourBoat.HealthBar.setCooldown(1f - (CooldownRemaining / ReloadTime));
		}

		//We can probably just drop our position if we die
		if (!bIsAlive())
        {
			gameObject.transform.position -= Vector3.up *3f* Time.deltaTime;
        }
	}
}
