using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Quick and dirty script to make the camera move so that I can see how the water behaves
public class BasicCameraMove : MonoBehaviour {

	void Update () {
		//Handle our camera base movement
		transform.position += transform.right * Input.GetAxis("Left Stick Horizontal");
		transform.position += transform.forward * Input.GetAxis("Left Stick Vertical");

		//Handle our look
		transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Right Stick Horizontal") * 50f * Time.deltaTime, Vector3.up);
		transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Right Stick Vertical") * 50f * Time.deltaTime, transform.right);
	}
}
