using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is for getting shot by the sniper cannon
//Needs to disable itself when the camera isn't in sniper mode
//Needs to relay damage back to the parent
//Needs to look way better than this MVP but fuckit
public class WeakPoint : MonoBehaviour {
	Collider ourCollider;
	float DamageMultiplier = 7f;
	public BoatSetup ourBoat;
	public GameObject WeakPointHitEffect;
	// Use this for initialization
	void Start () {
		ourCollider = gameObject.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		ourCollider.enabled = SailingCameraBehavior.Instance.CameraMode == SailingCameraBehavior.enCameraMode.SNIPE; //Only have the collider on if we're in sniper mode
	}

	public virtual void TakeDamage(float DamageAmount)
    {
		if (ourBoat)
        {
			ourBoat.TakeDamage(DamageMultiplier * DamageMultiplier);
			GameObject hitEffect = Instantiate(WeakPointHitEffect, gameObject.transform.position, Quaternion.identity, gameObject.transform.parent);

			gameObject.SetActive(false); //we managed to hit this point. We should play some SFX with that too
        }
    }
}
