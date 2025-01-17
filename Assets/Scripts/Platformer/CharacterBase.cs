using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CharacterBase : MonoBehaviour {
	public GameObject swordObject;	//What we'll simply be messing with
    public Animator swordAnimator;
    float restrikeTime;
    public enum enStrikeType { NULL, RED, GREEN, BLUE};
    public Material cubeMaterial;
    public float strikeStart = 0.5f;
    public float strikeEnd = 1.0f;

    public enStrikeType SelfStrikeType = enStrikeType.NULL;
    public CharacterBase enemyCharacter;
    
    void Start()
    {
        cubeMaterial = new Material(swordObject.GetComponent<Renderer>().material); //Clone our cube material
        swordObject.GetComponent<Renderer>().material = cubeMaterial;
    }

	public virtual void AnimateStrike(enStrikeType strikeType)
    {
        
        //Really we want our sword to do some indicating animation
        swordAnimator.SetTrigger("DoStrike");   //window start
        //Handle our strike
        StartCoroutine(MeleeCheckWait(strikeType));

        //So now we need to set our timings, and somehow see if we collide with our opponents sword

    }

    void setSwordColor(enStrikeType strikeType)
    {
        switch (strikeType)
        {
            case enStrikeType.NULL:
                cubeMaterial.SetColor("_Color", Color.white);
                break;
            case enStrikeType.RED:
                cubeMaterial.SetColor("_Color", Color.red);
                break;
            case enStrikeType.GREEN:
                cubeMaterial.SetColor("_Color", Color.green);
                break;
            case enStrikeType.BLUE:
                cubeMaterial.SetColor("_Color", Color.blue);
                break;
        }
    }

    IEnumerator MeleeCheckWait(enStrikeType strikeType)
    {

        SelfStrikeType = enStrikeType.NULL;
        setSwordColor(strikeType);
        yield return new WaitForSeconds(strikeStart);   //window start
        //So at this point we need a system that'll somehow check to see if our opponent is doing a particular action, within a particular window...
        SelfStrikeType = strikeType;    //So we're hitting here
        setSwordColor(SelfStrikeType);
        float strikeWindowStart = Time.time;
        while (Time.time < strikeWindowStart + strikeEnd - strikeStart)
        {
            yield return null;
            doHitCheck();
        }
        SelfStrikeType = enStrikeType.NULL;
        setSwordColor(SelfStrikeType);
    }

    void doHitCheck()
    {
        //This is a bit of a funky check as it'll entail potentially changing it's state as the gameplay goes on as the player strikes faster than the AI

    }
}
