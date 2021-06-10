using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHideShow : MonoBehaviour {

    private bool isLocked;

	// Use this for initialization
	void Start () {
	    SetCursorLock(false);
	    Cursor.lockState = CursorLockMode.None;
	}

	void SetCursorLock(bool isLocked){
	    this.isLocked = isLocked;
	    Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
	    Cursor.visible = !isLocked;
	}
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.L))
	        SetCursorLock(!isLocked);
	}
}
