using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class EnemyBoatsDetails
{
	public float BoatHealth = 200;
	public int VirutalCannons = 3;
}

//This script is the global controller for the sailing combat sequences. Hopefully it'll remain "global" while the sub-elements sort themselves out...
//Can address this script through the SailingGameController
public class SailingCombatController : MonoBehaviour {
	public enum enCombatState {  NONE, PLAYER, ENEMY, OPENING, FINISHED }
	public enCombatState CombatState = enCombatState.NONE;
	public GameObject BoatsLocation;
	[Space]
	[Header("Camera Locations")]
	public GameObject DeckCamera; //Populated at creation of player boat
	public GameObject CombatCamera;
	public GameObject EnemyDieCam;
	[Header("Boat Positions")]
	public GameObject PlayerBoatLocation;
	public GameObject EnemyBoatLocation;
	public GameObject EnemyBoatBase;
	[Space]
	[Header("Canvas settings")]
	public GameObject CombatCanvas;
	public GameObject EnemyHealthBarCanvas;
	public GameObject EnemyHealthBarPrefab;

	//We'll need a counter for how long each side gets to shoot at each other...
	protected Vector3 rotationRate = new Vector3(0, -1.5f, 0);

	//It makes a lot of sense to put different states on this class as we'll be handling things as such and everything else can just run as a slave
	public enum enPlayerWeapon { NONE, CANNON, CHAIN, SNIPE, MORTAR, BREAKER }
	public enPlayerWeapon SelectedWeapon = enPlayerWeapon.NONE;

	//Our wepon buttons should be a child of this class
	protected UI_WeaponSelectButton[] WeaponSelectButtons;
	protected WeaponBase[] PlayerWeapons;
	public TextMeshProUGUI weaponSelectTitle;
	protected float weaponSelectTime = 0;

	//Our enemy boats
	protected EnemyBoatCombatBehavior[] EnemyCountdownTimers;
	protected EnemyBoatPositionController ourBoatController;

	[Space]
	[Header("Setup Options")]
	public GameObject PlayerBoatPrefab;
	public List<GameObject> EnemyBoatsPrefab = new List<GameObject>();
	public List<EnemyBoatsDetails> EnemyBoatDetails = new List<EnemyBoatsDetails>();

	GameObject PlayerBoat;
	[HideInInspector] public BoatSetup PlayerBoatSetup;
	protected List<GameObject> EnemyBoats = new List<GameObject>();
	

	protected EnemyBoatPositionController ourEnemyPositionController;
	protected float OpeningStartTimer = 0f; //A small pause before we get to it

	public GameObject PlayerHealthBar;
	public Vector3 PlayerFinishPosition;
	public void SelectPlayerWeapon(enPlayerWeapon thisWeapon)
    {
		//We're breaking our combat phase
		if (thisWeapon == enPlayerWeapon.BREAKER)
        {			
			foreach(UI_WeaponSelectButton thisButton in WeaponSelectButtons)
            {
				if (thisButton.TargetWeapon == enPlayerWeapon.BREAKER)
                {
					UI_WeaponSelectButton BreakerButton = thisButton;

					if (BreakerButton.bCooldownReady)
					{	//Only do this if we're cooled down

						//We need to pass information back to our button here...
						ourBoatController.BreakEnemyVolleys();
						//We need to clear our count on the playerside
						PlayerBoatSetup.UsedBreaker();
					}
				}
            }


		}

		//We should do something clever here...
		if (SelectedWeapon != thisWeapon)
		{
			SelectedWeapon = thisWeapon;
			//We should get our system to display the weapon selection text
			SetWeaponSelectionText(thisWeapon.ToString());
		}
    }

	void SetWeaponSelectionText(string newName)
    {
		weaponSelectTitle.text = newName + " SELECTED!";
		weaponSelectTime = Time.time;
		weaponSelectTitle.gameObject.SetActive(true);
	}

	void SetupEncounter()
    {
		CombatState = enCombatState.OPENING;
		//Setup our player boat
		PlayerBoat = Instantiate(PlayerBoatPrefab, PlayerBoatLocation.transform.position, PlayerBoatLocation.transform.rotation, PlayerBoatLocation.transform);

		PlayerBoat.transform.localScale = Vector3.one;
		PlayerBoat.transform.localPosition = Vector3.zero;
		PlayerBoat.transform.localEulerAngles = Vector3.zero;

		PlayerBoatSetup = PlayerBoat.GetComponent<BoatSetup>();
		PlayerBoatSetup.SetHealthBar(PlayerHealthBar);

		Rigidbody PlayerRigidbody = PlayerBoat.GetComponent<Rigidbody>();
		Destroy(PlayerRigidbody); //We don't want this for this sequence as it'll cause headaches

		//Grab our deck camera off of our player boat
		CameraLocation[] PlayerCameras = PlayerBoat.GetComponentsInChildren<CameraLocation>();
		foreach (CameraLocation thisCamera in PlayerCameras)
        {
			if (thisCamera.CameraLocationType == CameraLocation.enCameraLocation.GUNNERY)
            {
				DeckCamera = thisCamera.gameObject;
            }
        }

		PlayerBoatPositionController BoatPositionController = gameObject.GetComponentInChildren<PlayerBoatPositionController>();
		BoatPositionController.playerBoat = PlayerBoat;

		//We need to setup all of our player weapons also
		ourEnemyPositionController = gameObject.GetComponentInChildren<EnemyBoatPositionController>();

		//setup our enemy boats
		for (int i=0; i<EnemyBoatsPrefab.Count; i++)
        {
			GameObject newEnemyBase = Instantiate(EnemyBoatBase, EnemyBoatLocation.transform);
			newEnemyBase.transform.localPosition = new Vector3(0, 0, i * 10f);
			newEnemyBase.transform.localEulerAngles = Vector3.zero;
			newEnemyBase.transform.localScale = Vector3.one;

			GameObject newBoat = Instantiate(EnemyBoatsPrefab[i], newEnemyBase.transform);
			newBoat.transform.localPosition = Vector3.zero;   //Put a step on the enemy boats so that they're staggered
			newBoat.transform.localEulerAngles = Vector3.zero;
			newBoat.transform.localScale = Vector3.one;

			Rigidbody NewBoatRigidbody = newBoat.GetComponent<Rigidbody>();
			Destroy(NewBoatRigidbody);

			//We need to have some more information about these boats in terms of when/what their reloads are
			newBoat.AddComponent<EnemyBoatCombatBehavior>();
			//PROBLEM: Setup our combat behaviour by setting its timers etc.

			ourEnemyPositionController.EnemyBoats.Add(newEnemyBase);
			ourEnemyPositionController.EnemyBoatBehavior.Add(new EnemyBoatBehaviors());

			BoatSetup enemyBoatSetup = newBoat.GetComponent<BoatSetup>();
			enemyBoatSetup.SetBoatDetails(EnemyBoatDetails[i].BoatHealth, EnemyBoatDetails[i].VirutalCannons);

			//Our enemy boats needs health bars...
			GameObject newHealthBar = Instantiate(EnemyHealthBarPrefab, EnemyHealthBarCanvas.transform.position, EnemyHealthBarCanvas.transform.rotation);
			newHealthBar.transform.SetParent(EnemyHealthBarCanvas.transform);
			newHealthBar.GetComponent<HealthBarBehavior>().trackingTarget = enemyBoatSetup.HealthBarLocation;

			enemyBoatSetup.SetHealthBar(newHealthBar);
        }
	}

	// Use this for initialization
	void Start () {
		//Here's where we setup our combat encounter. We could do with some clever command to pass through the information that we need to handle this
		SetupEncounter();

		//Get our weapon buttons
		WeaponSelectButtons = gameObject.GetComponentsInChildren<UI_WeaponSelectButton>();
		PlayerWeapons = gameObject.GetComponentsInChildren<WeaponBase>();
		ourBoatController = gameObject.GetComponentInChildren<EnemyBoatPositionController>();
		//Now lets go through and assign the buttons to the weapons so the classes can keep an eye on each other and know what's happening and when
		foreach (WeaponBase thisWeapon in PlayerWeapons)
        {
			if (!thisWeapon.AssignedSelectButton)
            {
				foreach (UI_WeaponSelectButton thisWeaponButton in WeaponSelectButtons)
                {
					if (thisWeapon.WeaponType == thisWeaponButton.TargetWeapon)
                    {
						thisWeapon.AssignedSelectButton = thisWeaponButton;
						break;
                    }
                }
            }
        }

		//We could do with getting all of our enemy boats, and the details around them
		EnemyCountdownTimers = gameObject.GetComponentsInChildren<EnemyBoatCombatBehavior>();
		//SailingCameraBehavior.Instance.TransitionToGameObject(DeckCamera);
		SailingCameraBehavior.Instance.SnapToGameObject(DeckCamera);    //Go to our combat camera position before we start the dodge sequence
		OpeningStartTimer = Time.time;
	}

	public void ReturnToPlayerControl()
	{
		//Reset all the boat timers
		foreach (EnemyBoatCombatBehavior ThisBoat in EnemyCountdownTimers)
		{
			ThisBoat.ResetCountdown();
			//Change the state to have the player's turn
		}
		CombatState = enCombatState.PLAYER; //This needs to have a position change call
		SailingCameraBehavior.Instance.TransitionToGameObject(DeckCamera);
		//We should move our player location back to zero least it's drifted from the dodging
		PlayerBoat.transform.DOLocalMove(Vector3.zero, 0.75f, false);
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time < weaponSelectTime + 2f)
        {
			float alpha = Mathf.Clamp01(weaponSelectTime + 2f - Time.time);
			weaponSelectTitle.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, alpha);
        }
        else
        {
			//Turn off our title
			weaponSelectTitle.gameObject.SetActive(false);
        }

		switch (CombatState)
        {
            case enCombatState.OPENING:
				if (Time.time > OpeningStartTimer + 3f) //Start the combat!
                {
					//Transition to the enemy combat state
					SailingCameraBehavior.Instance.TransitionToGameObject(CombatCamera);
					CombatState = enCombatState.ENEMY;
					ourBoatController.StartEnemyVolleys();
				}
				MoveCombatController();
                break;
			case enCombatState.PLAYER:
				WatchPlayerCombatLogic();
				CheckCombatEnded();
				MoveCombatController();
				break;
			case enCombatState.ENEMY:
				CheckCombatEnded();
				MoveCombatController();
				break;
			case enCombatState.FINISHED:
				break;
        }
	}

	public void MoveCombatController()
    {
		transform.localEulerAngles += rotationRate * Time.deltaTime;
	}

	public bool PlayerFiring()
    {
		bool bIsFiring = false;
		foreach (WeaponBase thisWeapon in PlayerWeapons)
		{
			if (thisWeapon.bInUse)
			{
				bIsFiring = true;
			}
		}
		return bIsFiring;
	}

	public void WatchPlayerCombatLogic()
	{
		//Debug.Log("Doing Combat Phase");

		//We need to keep an eye on the reload states for all of the enemy boats
		bool bAllComplete = true;
		foreach (EnemyBoatCombatBehavior ThisBoat in EnemyCountdownTimers)
		{
			if (ThisBoat.bIsAlive())    //Check to see that we're not dead
			{
				if (!ThisBoat.bIsReloaded)
				{
					bAllComplete = false;
				}
			}
		}
		//Debug.Log(bAllComplete);

		//Check and see if our player is firing a weapon
		bool bIsFiring = PlayerFiring();

		if (bAllComplete && !bIsFiring && CombatState != enCombatState.ENEMY)
		{
			//Transition to the enemy combat state
			SailingCameraBehavior.Instance.TransitionToGameObject(CombatCamera);
			CombatState = enCombatState.ENEMY;
			ourBoatController.StartEnemyVolleys();
		}
	}

	public void CheckCombatEnded() { 
		//Check to see if our player has died, or if all the enemies are dead
		bool bAllDead = true;
		foreach (EnemyBoatCombatBehavior ThisBoat in EnemyCountdownTimers)
		{
			if (ThisBoat.bIsAlive())
			{
				bAllDead = false;
			}
		}
		if (bAllDead)
        {
			EndCombatEncounter(true);
        }
        
		//Now check if the player has died (could be a critical target also)
		if (PlayerBoatSetup.BaseHealth <=0)	//Player has died. Lets just junk straight out
        {
			EndCombatEncounter(false);
        }
    }

    void EndCombatEncounter(bool bPlayerWon)
    {
		//We kind of need a flourish at the end of the sequence which will also allow us to display score and tidy everything up

		//So this should be a command call back to the SailingGameController I think
		//SailingGameController.Instance.GoIntoSailingMode(PlayerBoat.transform.position, PlayerBoat.transform.rotation);
		CombatState = enCombatState.FINISHED;
		if (bPlayerWon)
		{
			SailingCameraBehavior.Instance.TransitionToGameObject(EnemyDieCam);
		}
		ourBoatController.KillCoroutines(); //We don't want the coroutine to keep causing headaches

		//Destroy(gameObject); //Remove our battle encounter
		//PROBLEM: Need to bringup a UI to state that we've finished our encounter...or do we? I don't think we actually do for this
		//So I guess we can wait until the player presses a button, or just a pause
		if (bPlayerWon)
		{
			StartCoroutine(DelayCombatSequenceEnd());
		} else
        {
			StartCoroutine(HandlePlayerFail());
		}
    }

	IEnumerator HandlePlayerFail()
    {
		SailingCanvasController.Instance.SceneSwitchFade(1f, 1f);
		yield return new WaitForSeconds(1f);
		SailingCanvasController.Instance.SetAndDisplayMessage("YOU WERE DEFEATED!", 1f);
		SailingGameController.Instance.GoIntoSailingMode(PlayerFinishPosition, PlayerBoat.transform.rotation);
		SailingCanvasController.Instance.SceneSwitchFade(1f, 0f);
		Destroy(gameObject);
	}

	IEnumerator DelayCombatSequenceEnd()
    {
		yield return new WaitForSeconds(3);
		SailingGameController.Instance.GoIntoSailingMode(PlayerFinishPosition, PlayerBoat.transform.rotation);

		SailingCanvasController.Instance.SceneSwitchFade(1f, 1f);
		yield return new WaitForSeconds(1f);
		SailingCanvasController.Instance.SetAndDisplayMessage("ALL BOATS DEFEATED!", 1f);
		SailingCanvasController.Instance.SceneSwitchFade(1f, 0f);
		Destroy(gameObject);
	}
}
