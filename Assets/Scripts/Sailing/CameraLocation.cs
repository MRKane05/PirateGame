using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocation : MonoBehaviour {
	public enum enCameraLocation { NONE, GUNNERY, FOLLOW, COMBAT, FIXED }
	public enCameraLocation CameraLocationType = enCameraLocation.NONE;
}
