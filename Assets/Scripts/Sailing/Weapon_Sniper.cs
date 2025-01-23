using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The sniper weapon. Usage will be holding down shoulder and aiming with the thumbstick
public class Weapon_Sniper : WeaponBase {
	//Logically this is based off of the deck aiming....so...
	float Pitch, Yaw;
	float PitchMax = 30, YawMax = 40;   //I dunno, just something
	float AimSpeed = 50f;
	//public GameObject CannonsAimEmpty;
	public CanvasGroup SniperOverlayGroup;
	float SniperOverlayAlpha = 0;
	public float SniperDamage = 2f;
	public GameObject NonWeakpointHit;
	public override void WeaponUpdate()
    {
		//Control our Canvas group alpha here because this is just a MVP prototype
		if (SniperOverlayGroup)
        {
			SniperOverlayAlpha = Mathf.Lerp(SniperOverlayAlpha, SailingCameraBehavior.Instance.CameraMode == SailingCameraBehavior.enCameraMode.SNIPE ? 1f : 0f, Time.deltaTime * 4f);
			SniperOverlayGroup.alpha = SniperOverlayAlpha;
        }
    }

	public override void SelectedWeaponWatchFunction()
	{
		//Command start
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Left Shoulder") || Input.GetButtonDown("Right Shoulder")) && (WeaponState == enWeaponState.READY || WeaponState == enWeaponState.NONE))
		{
			bDoingFire = true;
			ClearButtonCooldown();
			WeaponState = enWeaponState.START;
		}

		//Command aim
		if ((Input.GetKey(KeyCode.Space) || Input.GetButton("Left Shoulder") || Input.GetButton("Right Shoulder")) && (WeaponState == enWeaponState.START || WeaponState == enWeaponState.AIMING))
		{
			bInUse = true;
			WeaponState = enWeaponState.AIMING;
			//Show our cannon aimer
			//Of course we also need to "aim" this...
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				Yaw -= AimSpeed * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.RightArrow))
			{
				Yaw += AimSpeed * Time.deltaTime;
			}
			Yaw = Mathf.Clamp(Yaw, -YawMax, YawMax);

			Yaw += Input.GetAxis("Left Stick Horizontal") * AimSpeed * Time.deltaTime;

#if !UNITY_EDITOR
			Yaw += Input.GetAxis("Right Stick Horizontal") * AimSpeed * Time.deltaTime;
#endif

			if (Input.GetKey(KeyCode.UpArrow))
			{
				Pitch -= AimSpeed * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				Pitch += AimSpeed * Time.deltaTime;
			}

			Pitch += Input.GetAxis("Left Stick Vertical") * AimSpeed * Time.deltaTime;
#if !UNITY_EDITOR
			Pitch += Input.GetAxis("Right Stick Vertical") * AimSpeed * Time.deltaTime;
#endif
			Pitch = Mathf.Clamp(Pitch, -PitchMax, PitchMax);

			Quaternion AimerAngle = Quaternion.Euler(Pitch, Yaw, 0f);

			//This needs to be piped through to our camera to take the instruction from this system, or exposed to get the information
			SailingCameraBehavior.Instance.SnipeCameraOffset = AimerAngle;
			SailingCameraBehavior.Instance.CameraMode = SailingCameraBehavior.enCameraMode.SNIPE;
		}

		if ((Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Left Shoulder") || Input.GetButtonUp("Right Shoulder")) && WeaponState == enWeaponState.AIMING)
		{
			WeaponState = enWeaponState.FINISHING;

			Quaternion AimerAngle = Quaternion.Euler(Pitch, Yaw, 0f);

			//This needs to be piped through to our camera to take the instruction from this system, or exposed to get the information
			SailingCameraBehavior.Instance.SnipeCameraOffset = AimerAngle;
			SailingCameraBehavior.Instance.CameraMode = SailingCameraBehavior.enCameraMode.SNIPE;
			DoSniperShot();
			//can do do a ray cast from our camera here?
			StartCoroutine(DoShotAndHold());
		}
	}

	public AudioSource ourAudio;
	public AudioClip sniperFireClip;

	void DoSniperShot()
    {
		if (!ourAudio)
        {
			ourAudio = gameObject.GetComponent<AudioSource>();
        }
		ourAudio.PlayOneShot(sniperFireClip);

		RaycastHit HitInfo;
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 1000.0f))
        {
			//Debug.Log("RayHit: " + HitInfo.collider.gameObject.name);
			WeakPoint HitWeakPoint = HitInfo.collider.gameObject.GetComponent<WeakPoint>();
			if (HitWeakPoint)
			{
				//Debug.LogError("Hit Weak Point");
				HitWeakPoint.TakeDamage(SniperDamage);
			}
			else
			{
				BoatSetup HitBoat = HitInfo.collider.gameObject.GetComponent<BoatSetup>();
				if (HitBoat)
				{
					HitBoat.TakeDamage(SniperDamage);
					GameObject hitEffect = Instantiate(NonWeakpointHit, HitInfo.point + HitInfo.normal*0.5f, Quaternion.identity, HitBoat.transform);
				}
			}
        }
	}

	IEnumerator DoShotAndHold()
    {
		bDoingFire = false;
		float StartTime = Time.time;
		while (Time.time < StartTime + 1f)
		{
			Quaternion AimerAngle = Quaternion.Euler(Pitch, Yaw, 0f);

			//This needs to be piped through to our camera to take the instruction from this system, or exposed to get the information
			SailingCameraBehavior.Instance.SnipeCameraOffset = AimerAngle;
			SailingCameraBehavior.Instance.CameraMode = SailingCameraBehavior.enCameraMode.SNIPE;
			yield return null;
		}

		DoAfterFire(2);
		SailingCameraBehavior.Instance.CameraMode = SailingCameraBehavior.enCameraMode.STANDARD;
	}
}
