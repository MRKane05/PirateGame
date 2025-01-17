using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialCombatController : SailingCombatController {

	bool bDodgeTutorialDisplayed = false;
	public GameObject DodgeTutorialObject;

	void DisplayDodgeTutorial()
	{
		if (bDodgeTutorialDisplayed) { return; }
		StartCoroutine(DelayDisplayDodgeTutorial());
	}
	
	IEnumerator DelayDisplayDodgeTutorial() {

		yield return new WaitForSeconds(1f);	//So that things syncronise a little better
		bDodgeTutorialDisplayed = true;
		DodgeTutorialObject.SetActive(true);
		//DOTween.To(() => DodgeTutorialCanvas.alpha, x => DodgeTutorialCanvas.alpha = x, 1f, 1f);
		Time.timeScale = 0.0001f;	//Pause our game
	}

	public void ReturnTimeScale()
    {
		Time.timeScale = 1f;
    }

	void Update()
	{
		switch (CombatState)
		{
			case enCombatState.OPENING:
				if (Time.time > OpeningStartTimer + 3f) //Start the combat!
				{
					//Transition to the enemy combat state
					SailingCameraBehavior.Instance.TransitionToGameObject(CombatCamera);
					CombatState = enCombatState.ENEMY;
					DisplayDodgeTutorial();
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
}
