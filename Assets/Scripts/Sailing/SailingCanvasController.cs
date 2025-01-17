using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SailingCanvasController : MonoBehaviour {
	private static SailingCanvasController instance = null;
	public static SailingCanvasController Instance { get { return instance; } }

	public CanvasGroup FadeCanvasGroup;
	public CanvasGroup MessageCanvasGroup;
	public TextMeshProUGUI MessageText;

	void Awake()
    {
		instance = this;	//Phucket
    }

	void Start()
    {
		//Fade in!
		FadeCanvasGroup.alpha = 1f;
		//Lets tween into the frame as an opening fade in
		DOTween.To(() => FadeCanvasGroup.alpha, x => FadeCanvasGroup.alpha = x, 0f, 1f);
    }

	public void SceneSwitchFade(float duration, float targetFadeAlpha)
    {
		//Sequence mySequence = DOTween.Sequence();
		DOTween.To(() => FadeCanvasGroup.alpha, x => FadeCanvasGroup.alpha = x, targetFadeAlpha, duration/2f);
		//mySequence.Append(DOTween.To(() => FadeCanvasGroup.alpha, x => FadeCanvasGroup.alpha = x, 0f, duration/2f));
	}

	public void SetAndDisplayMessage(string thisMessage, float duration)
    {
		MessageText.text = thisMessage;
		MessageCanvasGroup.alpha = 1f;
		Sequence mySequence = DOTween.Sequence();
		mySequence.PrependInterval(duration);
		mySequence.Append(DOTween.To(() => MessageCanvasGroup.alpha, x => MessageCanvasGroup.alpha = x, 0f, 0.5f));
	}
}
