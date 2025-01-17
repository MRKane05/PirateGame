using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailingGameController : MonoBehaviour {
	private static SailingGameController instance = null;
	public static SailingGameController Instance { get { return instance; } }

	public enum enGameMode { NONE, SAILING, COMBAT }
	public enGameMode GameplayMode = enGameMode.SAILING;

	public GameObject PlayerBoatPrefab;
	[HideInInspector] public GameObject PlayerBoat;

	[Space]
	[Header("Combat Components")]
	//public GameObject CombatControllerPrefab;
	public SailingCombatController CurrentCombatController;
	void Awake()
	{
		if (instance)
		{
			Debug.Log("Duplicate attempt to create SailingGameController");
			Debug.Log(gameObject.name);
			DestroyImmediate(gameObject);   //Ths isn't destroying the instance as expected...
			return; //cancel this
		}
		else
		{
			instance = this;
		}
	}

	void Start()
    {
		//We have to kick off our demo somewhere, lets make a player boat!
		GoIntoSailingMode(Vector3.zero, Quaternion.identity);
		//StartCoroutine(DelayCombatTest());
    }

	IEnumerator DelayCombatTest()
    {
		yield return new WaitForSeconds(3);
		//GoIntoCombatMode(Vector3.zero);
	}

	public void SetupCombatEngagement(GameObject CombatControllerPrefab, List<GameObject> EnemyBoatPrefabs, List<EnemyBoatsDetails> EnemyBoatDetails, Vector3 CombatCenter, Vector3 PlayerFinishPosition)
    {
		GameObject newCombatEncounter = Instantiate(CombatControllerPrefab, CombatCenter, Quaternion.identity);
		SailingCombatController newSailingCombatEncounter = newCombatEncounter.GetComponent<SailingCombatController>();
		newSailingCombatEncounter.EnemyBoatsPrefab = EnemyBoatPrefabs;
		newSailingCombatEncounter.EnemyBoatDetails = EnemyBoatDetails;
		newSailingCombatEncounter.PlayerFinishPosition = PlayerFinishPosition;

		GameplayMode = enGameMode.COMBAT;
		CurrentCombatController = newCombatEncounter.GetComponent<SailingCombatController>();
		Destroy(PlayerBoat);
	}

	public void GoIntoSailingMode(Vector3 playerPosition, Quaternion playerRotation)
    {
		GameplayMode = enGameMode.SAILING;
		//I assume we'll have to spawn our player
		PlayerBoat = Instantiate(PlayerBoatPrefab, playerPosition, playerRotation);
		//Camera has to move to behind the player boat
		CameraLocation[] Cameras = PlayerBoat.GetComponentsInChildren<CameraLocation>();
		foreach(CameraLocation thisCam in Cameras)
        {
			if (thisCam.CameraLocationType == CameraLocation.enCameraLocation.FOLLOW)
            {
				SailingCameraBehavior.Instance.SnapToFollowCam(thisCam.gameObject);
				SailingCameraBehavior.Instance.bDoingTween = false;	//To make sure we're shutting things down aggressively
			}
        }

		GameplayMode = enGameMode.SAILING;
    }

	public void GoIntoCombatMode(GameObject CombatControllerPrefab, Vector3 CombatCenter)	//And some detail of the enemies we'll be facing in this match...
    {
		GameObject newCombatEncounter = Instantiate(CombatControllerPrefab, CombatCenter, Quaternion.identity);
		GameplayMode = enGameMode.COMBAT;
		CurrentCombatController = newCombatEncounter.GetComponent<SailingCombatController>();
		Destroy(PlayerBoat);
		//Now we need to setup our different bits and pieces, and shift control to the combat encounter
    }
}
