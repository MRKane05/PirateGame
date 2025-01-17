using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Standard cannons. Usage will be holding down one of the shoulders and aiming with the thumbstick
public class Weapon_Cannons : WeaponBase {
	public GameObject CannonsAimerEmpty;
	float CannonsAimerAngle = 0f;
	float CannonsAimerMaxYaw = 40f;
	float AimSpeed = 50f;

	void Start()
	{
		CannonsAimerEmpty.SetActive(false);
	}

	public override void SelectedWeaponWatchFunction() {

		//Debug.Log("weapon tick");

		//Command start
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Left Shoulder") || Input.GetButtonDown("Right Shoulder")) && (WeaponState == enWeaponState.READY || WeaponState == enWeaponState.NONE))
		{
			WeaponState = enWeaponState.START;
			//Show our cannon aimer
			CannonsAimerEmpty.SetActive(true);
			ClearButtonCooldown();			
			//Debug.Log("Triggering Weapon Start");
		}
		//Command aim
		if ((Input.GetKey(KeyCode.Space) || Input.GetButton("Left Shoulder") || Input.GetButton("Right Shoulder")) && (WeaponState == enWeaponState.START || WeaponState == enWeaponState.AIMING))
		{
			bDoingFire = true;
			WeaponState = enWeaponState.AIMING;
			//Debug.Log("Weapon Aiming");
        }
		//Command end
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Left Shoulder") || Input.GetButtonUp("Right Shoulder")) && WeaponState == enWeaponState.AIMING)
		{
			WeaponState = enWeaponState.FINISHING;
			CannonsAimerEmpty.SetActive(false);
			//Calculate our forward Vector
			Vector3 FireVector = Quaternion.AngleAxis(CannonsAimerAngle, Vector3.up) * Vector3.forward;
			CannonsFire BoatCannons = gameObject.GetComponentInChildren<CannonsFire>();
			float SpeedMultiplier = 1f;	//PROBLEM: I'm unsure of this at present
			BoatCannons.FireCannons(FireVector, SpeedMultiplier);
			DoAfterFire(2);
		}
	}

	void LateUpdate()
	{


		if ((Input.GetKey(KeyCode.Space) || Input.GetButton("Left Shoulder") || Input.GetButton("Right Shoulder")) && WeaponState == enWeaponState.AIMING)
		{
			//Debug.Log(Input.GetAxis("Left Stick Horizontal"));
			CannonsAimerAngle += AimSpeed * Input.GetAxis("Left Stick Horizontal") * Time.deltaTime;
#if !UNITY_EDITOR
			CannonsAimerAngle += AimSpeed * Input.GetAxis("Right Stick Horizontal") * Time.deltaTime;
#endif
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				CannonsAimerAngle -= AimSpeed * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.RightArrow))
			{
				CannonsAimerAngle += AimSpeed * Time.deltaTime;
			}

			CannonsAimerAngle = Mathf.Clamp(CannonsAimerAngle, -CannonsAimerMaxYaw, CannonsAimerMaxYaw);
			CannonsAimerEmpty.transform.localEulerAngles = new Vector3(0f, CannonsAimerAngle, 0f);
		}
	}
}
