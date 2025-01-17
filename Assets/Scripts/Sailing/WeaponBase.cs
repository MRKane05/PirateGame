using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This, of course, handles the base functionality for the player weapons which should all (mostly) be similar in functionality
public class WeaponBase : MonoBehaviour {
	public enum enWeaponState {NONE, DISABLED, READY, START, AIMING, FINISHING, RELOADING };
	public enWeaponState WeaponState = enWeaponState.NONE;

	public SailingCombatController.enPlayerWeapon WeaponType;
	public UI_WeaponSelectButton AssignedSelectButton;
	public bool bInUse = false;
	public bool bDoingFire = false;

	void Update() {
		//We need to somehow check if this weapon can fire
		if (SailingGameController.Instance.CurrentCombatController.SelectedWeapon == WeaponType && SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.PLAYER && (AssignedSelectButton.bCooldownReady || (int)WeaponState > (int)enWeaponState.DISABLED)) {   //We kind of need a link to our weapon button for ease of use here
			SelectedWeaponWatchFunction();
		}
		WeaponUpdate();

		//Keep an eye on our button state and set reloaded if we're active again. This is terrible coding
		if (AssignedSelectButton.bCooldownReady && (WeaponState == enWeaponState.NONE || WeaponState == enWeaponState.RELOADING))
        {
			WeaponState = enWeaponState.READY;
        }
	}

	public void ReloadComplete()
    {
		WeaponState = enWeaponState.READY;
    }

	public virtual void WeaponUpdate()
    {

    }

	//This'll be the core function stuff for using our weapon (whatever it'll be)
	public virtual void SelectedWeaponWatchFunction() {

	}

	public void ClearButtonCooldown()
    {
		AssignedSelectButton.ResetCooldown();
	}

	//For when our weapon has been fired. This will mostly be responsible for resetting the cooldown
	public void DoAfterFire(float DelayTime)
	{
		AssignedSelectButton.ResetCooldown();
		StartCoroutine(WaitForCannonballs(2f));
		bDoingFire = false;
	}

	IEnumerator WaitForCannonballs(float DelayTime) { 
		yield return new WaitForSeconds(DelayTime);
		//Really we need a 3 second delay before the in use flag changes as that's the lifespan of our cannonballs? Or can we just junk everything?
		bInUse = false;
		WeaponState = enWeaponState.RELOADING;
    }
}
