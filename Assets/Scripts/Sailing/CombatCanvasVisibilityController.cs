using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatCanvasVisibilityController : MonoBehaviour {
	public SailingCombatController.enCombatState VisibleState = SailingCombatController.enCombatState.NONE;
	CanvasGroup ourCanvasGroup;
	void Start () {
		ourCanvasGroup = gameObject.GetComponent<CanvasGroup>();
	}
	
	
	void Update () {
		//Fade our canvas group if we're not in the correct state
		ourCanvasGroup.alpha = Mathf.Lerp(ourCanvasGroup.alpha, VisibleState == SailingGameController.Instance.CurrentCombatController.CombatState ? 1f : 0f, Time.deltaTime * 5f);
	}
}
