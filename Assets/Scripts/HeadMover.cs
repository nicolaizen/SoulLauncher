using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadMover : MonoBehaviour {

	void LateUpdate () {
		transform.rotation = transform.parent.parent.Find("FPS Camera").rotation;
	}
}
