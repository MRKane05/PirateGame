using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehavior : MonoBehaviour {
    public GameObject CannonFireEffect;
    public void FireCannon(GameObject Instigator, GameObject CannonballPrefab, Vector3 StartDirection, GameObject BoatsLocation, float DamageAmount, float SpeedMultiplier, float MaxDelay)
    {
        StartCoroutine(DelayFireCannon(Instigator, CannonballPrefab, StartDirection, BoatsLocation, DamageAmount, SpeedMultiplier, MaxDelay));
    }

    IEnumerator DelayFireCannon(GameObject Instigator, GameObject CannonballPrefab, Vector3 StartDirection, GameObject BoatsLocation, float DamageAmount, float SpeedMultiplier, float MaxDelay) {
        yield return new WaitForSeconds(Random.Range(0f, MaxDelay));
        //Do sound
        //Do fire effect
        GameObject FireEffect = Instantiate(CannonFireEffect, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        GameObject newCannonball = Instantiate(CannonballPrefab, gameObject.transform.position, Quaternion.identity, BoatsLocation.transform);
        newCannonball.transform.localScale = Vector3.one;
        CannonBallBehavior cannonballScript = newCannonball.GetComponent<CannonBallBehavior>();
        cannonballScript.Setup(StartDirection, 30 * SpeedMultiplier, 10, Instigator);
    }
}
