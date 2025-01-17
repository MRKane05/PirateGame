using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class could probably be responsible for driving all combat actions keeping the boat as a holder and just a visual reference
public class PlayerBoatPositionController : MonoBehaviour {
    public AnimationCurve DodgeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1f), new Keyframe(0.75f, 1f), new Keyframe(0, 0));
    float DodgeDuration = 2f;
    float DodgeDistance = 25f;
    Vector3 BoatStartPosition = Vector3.zero;
    public GameObject playerBoat;

    bool bIsDodging = false;
    void Update()
    {
        //This is close enough for the moment!
        if (SailingGameController.Instance.CurrentCombatController.CombatState == SailingCombatController.enCombatState.ENEMY && !bIsDodging)
        {

            bool bDodgeRight = false;
            bool bDodgeLeft = false;
            if (Input.GetAxis("Left Stick Horizontal") > 0.5f || Input.GetButton("Dright"))
            {
                bDodgeRight = true;
            }
            if (Input.GetAxis("Left Stick Horizontal") < -0.5f || Input.GetButton("Dleft"))
            {
                bDodgeLeft = true;
            }
#if !UNITY_EDITOR
            if (Input.GetAxis("Right Stick Horizontal") > 0.5f)
            {
                bDodgeRight = true;
            }
            if (Input.GetAxis("Right Stick Horizontal") < -0.5f)
            {
                bDodgeLeft = true;
            }
#endif

            //Debug.Log(Input.GetAxis("Left Stick Horizontal"));
            //Then our boat can move!
            if (Input.GetKeyDown(KeyCode.RightArrow) || bDodgeRight)
            {
                //Do a move to the right
                StartCoroutine(DodgePlayerBoat(1f));
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || bDodgeLeft)
            {
                //Do a move to the left
                StartCoroutine(DodgePlayerBoat(-1f));
            }
        }
    }
    void LateUpdate() { 
        if (!bIsDodging && playerBoat)
        {
            playerBoat.transform.localPosition = Vector3.Lerp(playerBoat.transform.localPosition, Vector3.zero, Time.deltaTime * 2f); //PROBLEM: We've got a funny drift and I'm simply hacking a solution
        }
	}

    IEnumerator DodgePlayerBoat(float DodgeDirection)
    {
        bIsDodging = true;
        //This'll handle the dodge, but also control other effects like the sails/wind etc.
        float DodgeStartTime = Time.time;
        BoatStartPosition = playerBoat.transform.localPosition;
        while (Time.time < DodgeStartTime + DodgeDuration)
        {
            float DodgeT = (Time.time - DodgeStartTime) / DodgeDuration;
            float DodgeEval = DodgeCurve.Evaluate(DodgeT);
            playerBoat.transform.localPosition = BoatStartPosition + Vector3.forward * DodgeEval * DodgeDistance * DodgeDirection;

            yield return null;
        }

        bIsDodging = false;
    }
}
