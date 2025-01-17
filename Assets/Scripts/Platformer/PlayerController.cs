using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase {

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
			AnimateStrike(enStrikeType.RED);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			AnimateStrike(enStrikeType.GREEN);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			AnimateStrike(enStrikeType.BLUE);
		}
	}
}
