using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Chain cannon. Usage will be pressing on the screen to aim where the shots go
public class Weapon_ChainGun : WeaponBase
{
    int roundsInCanister = 6;
    float refireTime = 0.6f;

    public GameObject ChainCannonballPrefab;
    public GameObject CannonFireEffect;
    public GameObject PlayerBoat;

    public override void SelectedWeaponWatchFunction()
    {
        if (Input.GetMouseButtonDown(0) && WeaponState == enWeaponState.READY)    
        {
            bDoingFire = true;
            ClearButtonCooldown();
            StartCoroutine(FireChainCannon());
        }
    }

    IEnumerator FireChainCannon()
    {
        bInUse = true;
        WeaponState = enWeaponState.AIMING;
        //CannonsFire BoatCannons = gameObject.GetComponentInChildren<CannonsFire>();
        for (int i = 0; i < roundsInCanister; i++)
        {
            //Lets use the first cannon the BoatCannons list as the point that we fire out of
            Vector3 startPosition = gameObject.transform.position; // BoatCannons.ourBoat.NearCannons[0].transform.position;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Convert our firing direction to allow for cannonballs moving along local space (still need to have something that'll handle physics here)
            Vector3 FireDirection = SailingGameController.Instance.CurrentCombatController.BoatsLocation.transform.InverseTransformDirection(ray.direction);

            GameObject FireEffect = Instantiate(CannonFireEffect, transform.position, transform.rotation, transform);

            GameObject newCannonball = Instantiate(ChainCannonballPrefab, gameObject.transform.position, Quaternion.identity, SailingGameController.Instance.CurrentCombatController.BoatsLocation.transform);
            newCannonball.transform.localScale = Vector3.one;
            CannonBallBehavior cannonballScript = newCannonball.GetComponent<CannonBallBehavior>();
            cannonballScript.Setup(FireDirection, 30, 10, PlayerBoat);

            yield return new WaitForSeconds(refireTime);
        }
        DoAfterFire(2);
    }
}
