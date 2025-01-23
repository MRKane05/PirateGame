using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Mortar : WeaponBase
{
	public GameObject MortarAimingObject;
	public GameObject MortarBallPrefab;
	public GameObject PlayerBoat;

	Vector3 AimingObjectStart = Vector3.zero;
	float AimMoveSpeed = 30f;
	public float MortarSpawnRadius = 7f;
	float MortarSpawnDelay = 0.5f;
	float MortarCameraDelay = 3f;
	public int NumberOfMortarShots = 6;

	void Start()
	{
		AimingObjectStart = MortarAimingObject.transform.localPosition;   //Set this as we'll reuse it as necessary
		MortarAimingObject.SetActive(false);
	}

	public override void SelectedWeaponWatchFunction()
	{
		//Command start
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Left Shoulder") || Input.GetButtonDown("Right Shoulder")) && (WeaponState == enWeaponState.READY || WeaponState == enWeaponState.NONE))
		{
			WeaponState = enWeaponState.START;
			bDoingFire = true;
			ClearButtonCooldown();
			bInUse = true;
			MortarAimingObject.transform.localPosition = AimingObjectStart;
			//We need to transition our camera to the combat camera position to give an overview of the mortar positioning
			SailingCameraBehavior.Instance.TransitionToGameObject(SailingGameController.Instance.CurrentCombatController.CombatCamera);
			MortarAimingObject.SetActive(true);
		}

		//Command aim
		if ((Input.GetKey(KeyCode.Space) || Input.GetButton("Left Shoulder") || Input.GetButton("Right Shoulder")) && (WeaponState == enWeaponState.START || WeaponState == enWeaponState.AIMING))
		{
			bDoingFire = true;
			WeaponState = enWeaponState.AIMING;
			//Show our cannon aimer
			//Of course we also need to "aim" this...
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				MortarAimingObject.transform.localPosition -= Vector3.forward * AimMoveSpeed * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.RightArrow))
			{
				MortarAimingObject.transform.localPosition += Vector3.forward * AimMoveSpeed * Time.deltaTime;
			}
			//Put a clamp in for the mortar aimer
			MortarAimingObject.transform.localPosition += Input.GetAxis("Left Stick Horizontal") * Vector3.forward * AimMoveSpeed * Time.deltaTime;
#if !UNITY_EDITOR
			MortarAimingObject.transform.localPosition += Input.GetAxis("Right Stick Horizontal") * Vector3.forward * AimMoveSpeed * Time.deltaTime;
#endif


			if (Input.GetKey(KeyCode.UpArrow))
			{
				MortarAimingObject.transform.localPosition -= Vector3.right * AimMoveSpeed * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				MortarAimingObject.transform.localPosition += Vector3.right * AimMoveSpeed * Time.deltaTime;
			}
			MortarAimingObject.transform.localPosition += Input.GetAxis("Left Stick Vertical") * Vector3.right * AimMoveSpeed * Time.deltaTime;
#if !UNITY_EDITOR
			MortarAimingObject.transform.localPosition += Input.GetAxis("Right Stick Vertical") * Vector3.right * AimMoveSpeed * Time.deltaTime;
#endif
		}

		if ((Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Left Shoulder") || Input.GetButtonUp("Right Shoulder")) && WeaponState == enWeaponState.AIMING)
		{
			WeaponState = enWeaponState.FINISHING;
			StartCoroutine(DoMortarStrike());
        }
	}

	public AudioSource ourAudio;
	public AudioClip mortarFireClip;

	IEnumerator DoMortarStrike()
    {

		if (!ourAudio)
		{
			ourAudio = gameObject.GetComponent<AudioSource>();
		}
		ourAudio.PlayOneShot(mortarFireClip);

		bDoingFire = false;	//End this early so that the player cannot spam this weapon
		yield return new WaitForSeconds(MortarSpawnDelay);
		//we need to spawn mortar around the strike area
		Vector3 SpawnLocation = MortarAimingObject.transform.position;
		for (int i=0; i<NumberOfMortarShots; i++)
        {
			Vector3 newSpawnPos = SpawnLocation + MortarAimingObject.transform.right * Random.RandomRange(-MortarSpawnRadius, MortarSpawnRadius) + MortarAimingObject.transform.forward * Random.RandomRange(-MortarSpawnRadius, MortarSpawnRadius) + Vector3.up;
			newSpawnPos += Vector3.up * Random.Range(50f, 70f); //So that there's variation on our dropping balls

			//Of course these have to go down...
			GameObject newCannonball = Instantiate(MortarBallPrefab, newSpawnPos, Quaternion.identity);
			newCannonball.transform.SetParent(MortarAimingObject.transform);
			CannonBallBehavior ballBehavior = newCannonball.GetComponent<CannonBallBehavior>();
			ballBehavior.Setup(-Vector3.up, 30f, 7f, PlayerBoat);
		}

		yield return new WaitForSeconds(MortarCameraDelay); //So we can see the end results take effect
		DoAfterFire(2);
		MortarAimingObject.SetActive(false);
		SailingCameraBehavior.Instance.TransitionToGameObject(SailingGameController.Instance.CurrentCombatController.DeckCamera);
		SailingCameraBehavior.Instance.CameraMode = SailingCameraBehavior.enCameraMode.STANDARD;
	}
}
