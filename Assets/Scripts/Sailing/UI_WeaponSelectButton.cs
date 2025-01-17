using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A handler class that'll do all the necessary stuff for the different weapon select buttons
public class UI_WeaponSelectButton : MonoBehaviour {
	public Image ReadyFillImage;
	public Image IconBorder;
	public SailingCombatController.enPlayerWeapon TargetWeapon;
	public SailingCombatController.enCombatState TargetGameState = SailingCombatController.enCombatState.PLAYER;
	protected Color IconSelected = Color.green;
	protected Color IconPlay = Color.black;
	public float WeaponCooldown = 15f;
	public float StartAdvantage = 0f;
	float CooldownRemaining = 0;
	public bool bCooldownReady = false;

	public bool bScriptDriven = false;

	[Space]
	[Header("Tutorial stuff")]
	public GameObject WeaponTutorialPanel;
	bool bHasHadTutorial = false;

	//We're going to need something to define what our cooldown is, and to take forward/back commands in that respect

	// Use this for initialization
	void Start()
    {
		StartResetCooldown();
    }
	public void StartResetCooldown()
	{
		CooldownRemaining = WeaponCooldown - StartAdvantage;
	}

	public void ResetCooldown()
    {
		CooldownRemaining = WeaponCooldown;
    }

	public void SelectWeapon()
    {
		if (SailingGameController.Instance.CurrentCombatController && SailingGameController.Instance.CurrentCombatController.CombatState == TargetGameState)
        {
			SailingGameController.Instance.CurrentCombatController.SelectPlayerWeapon(TargetWeapon);
		}
    }

	void CheckButtonColours()
    {
		//PROBLEM: This is terribly lazy...
		IconBorder.color = SailingGameController.Instance.CurrentCombatController.SelectedWeapon == TargetWeapon ? IconSelected : IconPlay;
    }

	public void SetFillAmount(float toThis)
    {
		//Debug.Log("Fill Set: " + toThis.ToString());
		//float fillAmount = 1f - Mathf.Clamp01(CooldownRemaining / WeaponCooldown);
		if (!bScriptDriven) //Consider this to be a bonus effect, somehow?
		{

		}
		ReadyFillImage.fillAmount = toThis;
		bCooldownReady = toThis >= 1f;
		//Pause to go into tutorial mode
		if (toThis >= 1f && !bHasHadTutorial && WeaponTutorialPanel)
		{
			bHasHadTutorial = true;
			WeaponTutorialPanel.SetActive(true);
			Time.timeScale = 0.0001f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//We need to tick our cooldown while it's our turn. That isn't written in yet of course.
		if (SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.PLAYER && !bScriptDriven)
        {
			CooldownRemaining -= Time.deltaTime;
			float fillAmount = 1f - Mathf.Clamp01(CooldownRemaining / WeaponCooldown);
			ReadyFillImage.fillAmount = fillAmount;
			bCooldownReady = fillAmount >= 1f;

			if (fillAmount >= 1f && !bHasHadTutorial && WeaponTutorialPanel && !SailingGameController.Instance.CurrentCombatController.PlayerFiring())
			{
				bHasHadTutorial = true;
				WeaponTutorialPanel.SetActive(true);
				Time.timeScale = 0.0001f;
			}

		}
		
		CheckButtonColours();	
	}
}
