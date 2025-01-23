using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatCanvasVisibilityController : MonoBehaviour {
	public SailingCombatController.enCombatState VisibleState = SailingCombatController.enCombatState.NONE;
	CanvasGroup ourCanvasGroup;
	bool bCanvasVisible = true;
	void Start () {
		ourCanvasGroup = gameObject.GetComponent<CanvasGroup>();
	}
	
	
	void Update () {
		//Fade our canvas group if we're not in the correct state
		ourCanvasGroup.alpha = Mathf.Lerp(ourCanvasGroup.alpha, VisibleState == SailingGameController.Instance.CurrentCombatController.CombatState ? 1f : 0f, Time.deltaTime * 5f);
		//PROBLEM: This is terrible hacky design that'll need a better handler
		if (bCanvasVisible) {
			if (ourCanvasGroup.alpha < 0.001f)  //We need to disable our children
			{
				bCanvasVisible = false;
				foreach (Transform child in gameObject.transform)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
		if (!bCanvasVisible)
        {
			if (ourCanvasGroup.alpha > 0.001f)  //We need to disable our children
			{
				bCanvasVisible = true;
				foreach (Transform child in gameObject.transform)
				{
					child.gameObject.SetActive(true);
				}
			}
		}
	}
}
