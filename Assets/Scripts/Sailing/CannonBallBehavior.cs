using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehavior : MonoBehaviour {

    public GameObject hitEffectPrefab;
    public GameObject splashEffectPrefab;

    public float DamageAmount = 7f;
    float Speed = 30f;
    float Damage = 10f;
    Vector3 Direction = Vector3.forward;    //Set for the moment
    GameObject Instigator;

    public void Setup(Vector3 newDirection, float newSpeed, float newDamage, GameObject newInstigator)
    {
        Direction = newDirection;
        Speed = newSpeed;
        Damage = newDamage;
        Instigator = newInstigator;
        Destroy(gameObject, 3f);    //Quick die function
    }

    //So the ball needs to move along a local axis in order for everything to line up as expected, and we'll have some "drop" functionality also
    void LateUpdate()
    {
        transform.localPosition += Direction * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Instigator) return; //Don't hit ourselves

        //Speed = Speed * -1;
        BoatSetup HitBoat = other.gameObject.GetComponent<BoatSetup>();
        if (HitBoat)
        {
            //Play a hit effect
            GameObject hitEffect = Instantiate(hitEffectPrefab, gameObject.transform.position, Quaternion.identity, other.gameObject.transform);
            //Play a hit sound
            //Apply damage to target boat
            HitBoat.TakeDamage(DamageAmount);
            Destroy(gameObject);    //Destroy our cannonball
        }
    }
}
