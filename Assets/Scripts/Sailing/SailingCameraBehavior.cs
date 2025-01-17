using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransitionTo
{
	public GameObject targetObject;
	public float Yaw = 0;
	public float Pitch = 0;
	public float targetDistance = 20f;
	public float transitionTime = 0.5f;
	public bool bTransitionComplete = false;

	public TransitionTo(GameObject newTargetobject, float newYaw, float newPitch, float newTargetDistance, float newTransitionTime)
    {
		targetObject = newTargetobject;
		Yaw = newYaw;
		Pitch = newPitch;
		targetDistance = newTargetDistance;
		transitionTime = newTransitionTime;
    }
}

//This camera will have a couple of different modes that it'll have to operate on
public class SailingCameraBehavior : MonoBehaviour {
	private static SailingCameraBehavior instance = null;
	public static SailingCameraBehavior Instance { get { return instance; } }

	public enum enCameraMode { NONE, STANDARD, FREE, CINEMATIC, SNIPE }
	public enCameraMode CameraMode = enCameraMode.STANDARD;

	public float Yaw = 0;
	public float Pitch = 15f;
	public float CamDistance = 20f;
	public GameObject CameraTarget;

	public Queue<TransitionTo> CameraTransitions = new Queue<TransitionTo>();
	public bool bDoingTween = false;
	public AnimationCurve CameraTransitionCurve = new AnimationCurve();

	public GameObject closeCamera, farCamera;

	float baseFOV = 60f;

	public Quaternion SnipeCameraOffset = Quaternion.identity;

	void Awake()
	{
		if (instance)
		{
			Debug.Log("Duplicate attempt to create SailingCameraBehavior");
			Debug.Log(gameObject.name);
			DestroyImmediate(gameObject);   //Ths isn't destroying the instance as expected...
			return; //cancel this
		}
		else
		{
			instance = this;
			//DontDestroyOnLoad(this); //this is about the only thing that's not cycled around.
		}
		//Grab our default settings
		baseFOV = gameObject.GetComponent<Camera>().fieldOfView;
	}

	public void setCameraTarget(GameObject toThis)
    {
		CameraTarget = toThis;
    }
	
	// Update is called once per frame
	void LateUpdate () {
		if (SailingGameController.Instance)
        {
			switch (SailingGameController.Instance.GameplayMode)
            {
				case SailingGameController.enGameMode.SAILING:
					FollowCamera();
					break;
				case SailingGameController.enGameMode.COMBAT:
					CombatCamera();
					break;
            }
        }
	}

	float CameraRotation = 0f;

	void FollowCamera()
    {
		if (!CameraTarget || bDoingTween) return;

		//Debug.Log(Input.GetAxis("Right Stick Horizontal"));
#if !UNITY_EDTIOR
		CameraRotation += Input.GetAxis("Right Stick Horizontal") * Time.deltaTime;
#endif
		//NB: We should have better camera controls...

		Quaternion RotationQuat = Quaternion.AngleAxis(CameraRotation, Vector3.up);
		//Lets begin by simply offsetting our camera
		gameObject.transform.position = CameraTarget.transform.position - RotationQuat*CameraTarget.transform.forward * CamDistance;
		//Now I guess we need to get our look at stuff :/
		gameObject.transform.LookAt(CameraTarget.transform); //Who cares right now
    }

	void CombatCamera()
    {
		if (!CameraTarget || bDoingTween) return;

		if (CameraMode == enCameraMode.STANDARD)
		{
			gameObject.transform.position = CameraTarget.transform.position;
			gameObject.transform.rotation = CameraTarget.transform.rotation;
		}
		if (CameraMode == enCameraMode.SNIPE)
        {
			//We'll have to have our camera zoom in...
			gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gameObject.GetComponent<Camera>().fieldOfView, 40f, Time.deltaTime * 2f);
			//I expect we'll get an offset sent through...
			gameObject.transform.position = CameraTarget.transform.position;
			gameObject.transform.rotation = CameraTarget.transform.rotation;
			gameObject.transform.rotation *= SnipeCameraOffset;
			//We need a camera that'll hang around our position, zoom in a bit, and have a Goldeneye style cursor control attached to it...
			//Of course this requies: UI elements, control scripts, interaction scripts. Fun times, I've got no idea where I'd put these controls...
		} else
        {			gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gameObject.GetComponent<Camera>().fieldOfView, baseFOV, Time.deltaTime * 2f);

		}
	}

	void Update()
    {
		if (CameraTransitions.Count > 0 && !bDoingTween)
        {
			TransitionTo nextTransition = CameraTransitions.Dequeue();
			StartCoroutine(DoTransition(nextTransition));
        }
    }

	public void TransitionToGameObject(GameObject TargetObject)
    {
		if (TargetObject == null)
        {
			Debug.Log("Target Object not set for camera transition");
        }
		TransitionTo newTransition = new TransitionTo(TargetObject, 0, 0, 0, 0.5f);
		CameraTransitions.Enqueue(newTransition);
	}

	public void SnapToGameObject(GameObject TargetObject)
    {
		if (TargetObject == null)
		{
			Debug.Log("Target Object not set for camera transition");
		}
		CameraTarget = TargetObject;
	}

	public void SnapToFollowCam(GameObject TargetCameraPosition)
    {
		StopAllCoroutines();	//Lazy hack here
		if (TargetCameraPosition == null)
		{
			Debug.Log("Target Object not set for snap follow camera transition");
		}
		CameraMode = enCameraMode.STANDARD;
		CameraTarget = TargetCameraPosition;
	}

	IEnumerator DoTransition(TransitionTo newCameraTarget)
    {
		bDoingTween = true;
		float lerpAlpha = 0f;
		float lerpStart = Time.time;
		float lerpFactor = 0f;
		GameObject startObject = CameraTarget;
		//There's a possiblity that startObject is null
		if (startObject)
		{
			while (Time.time < lerpStart + newCameraTarget.transitionTime)
			{
				lerpAlpha = (Time.time - lerpStart) / newCameraTarget.transitionTime;
				//Would be cool if this was a slerp or something
				lerpFactor = CameraTransitionCurve.Evaluate(lerpAlpha);
				gameObject.transform.position = Vector3.Lerp(startObject.transform.position, newCameraTarget.targetObject.transform.position, lerpFactor);
				gameObject.transform.rotation = Quaternion.Slerp(startObject.transform.rotation, newCameraTarget.targetObject.transform.rotation, lerpFactor);
				yield return null;
			}
		} else
        {
			Vector3 startPosition = gameObject.transform.position;
			Quaternion startRotation = gameObject.transform.rotation;
			while (Time.time < lerpStart + newCameraTarget.transitionTime)
			{
				lerpAlpha = (Time.time - lerpStart) / newCameraTarget.transitionTime;
				//Would be cool if this was a slerp or something
				lerpFactor = CameraTransitionCurve.Evaluate(lerpAlpha);
				gameObject.transform.position = Vector3.Lerp(startPosition, newCameraTarget.targetObject.transform.position, lerpFactor);
				gameObject.transform.rotation = Quaternion.Slerp(startRotation, newCameraTarget.targetObject.transform.rotation, lerpFactor);
				yield return null;
			}
		}

		CameraTarget = newCameraTarget.targetObject;
		bDoingTween = false;
    }

	void TransitionCamera(TransitionTo newCameraTarget)
    {
		bDoingTween = true;
		//Basically we'll want to have our camera do a transition here, we can probably use the DG.Tweening for all of this
		//Holy crap that didn't work.
		DOTween.To(() => gameObject.transform.position, x => gameObject.transform.position = x, newCameraTarget.targetObject.transform.position, newCameraTarget.transitionTime);
		DOTween.To(() => gameObject.transform.eulerAngles, x => gameObject.transform.eulerAngles = x, newCameraTarget.targetObject.transform.eulerAngles, newCameraTarget.transitionTime);
    }
}
