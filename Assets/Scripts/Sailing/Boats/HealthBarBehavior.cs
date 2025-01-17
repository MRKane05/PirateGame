using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehavior : MonoBehaviour {
	public CanvasGroup ourCanvasGroup;
	public Transform HealthDisplayTransform;
	public Image CooldownTimerFill;
	public GameObject trackingTarget;

	RectTransform ourParent, ourRect;

	// Use this for initialization
	void Start () {
		ourParent = transform.transform.gameObject.GetComponent<RectTransform>();
		ourRect = gameObject.GetComponent<RectTransform>();
		if (ourCanvasGroup) { ourCanvasGroup.alpha = 1; } //for the moment
	}
	
	public void setBarHealth(float newHealth)
    {
		HealthDisplayTransform.localScale = new Vector3(newHealth, 1f, 1f);
    }

	public void setCooldown(float newCooldown)
    {
		CooldownTimerFill.fillAmount = newCooldown;
    }

	void LateUpdate()
    {
		if (trackingTarget)
        {
			Vector3 canvasPosition = Camera.main.WorldToScreenPoint(trackingTarget.transform.position);
			ourRect.anchoredPosition = canvasPosition;
        }
    }

	public void RemoveBar()
    {
		if (ourCanvasGroup) { ourCanvasGroup.alpha = 0; }
		Destroy(gameObject);
    }

}
