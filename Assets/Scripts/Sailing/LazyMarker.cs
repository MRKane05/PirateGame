using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is just a world position label that we're not expected to care about all that much
public class LazyMarker : MonoBehaviour {

	public SailingGameController.enGameMode VisibleStateMode = SailingGameController.enGameMode.SAILING;
	CanvasGroup ourCanvas;
	float LabelAlpha = 0f;
	void Start()
    {
		ourCanvas = gameObject.GetComponentInChildren<CanvasGroup>();
    }
	// Update is called once per frame
	void Update () {
		//We need to face our camera
		gameObject.transform.LookAt(Camera.main.transform.position, Vector3.up);
		//Scale according to distance
		gameObject.transform.localScale = Vector3.one * Vector3.Distance(Camera.main.transform.position, gameObject.transform.position) / 200f;
		
		//Quick and dirty fade details
		if (SailingGameController.Instance.GameplayMode == VisibleStateMode)
		{
			if (gameObject.transform.localScale.x > 0.6f)
			{
				LabelAlpha = 1f;
			}
			else
			{
				LabelAlpha = 0f;
			}
		} else
        {
			LabelAlpha = 0f;
        }
		ourCanvas.alpha = Mathf.Lerp(ourCanvas.alpha, LabelAlpha, Time.deltaTime * 4f);		
	}
}
