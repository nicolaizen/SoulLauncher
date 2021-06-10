using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarpBallController : MonoBehaviour {

    private Rigidbody rb;

    public float warpBallInitSpeed = 20f;
    public float warpBallSpawnHeight = 1.3f;
    public float warpBallSpawnDistance = 1.3f;

    public GameObject creator;

	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	// void Update () {}

	public void InitiateByCreator(GameObject creator, float rotationAxisX, float rotationAxisY){
	    Start();
	    this.creator = creator;
	    SetSpawnPosition(rotationAxisX, rotationAxisY);
	    SetInitTrajectory();
	    //SetCreatorAsParent();
	    SetGlobalWarpBallPoolAsParent();
	}

    public void LaunchFromCreator(float rotationAxisX, float rotationAxisY){
        SetSpawnPosition(rotationAxisX, rotationAxisY);
	    SetInitTrajectory();
        SetActive(true);
    }

	void SetSpawnPosition(float rotationAxisX, float rotationAxisY){
	    float posX = creator.transform.position.x + (rotationAxisX * warpBallSpawnDistance);
        float posY = creator.transform.position.y + warpBallSpawnHeight;
        float posZ = creator.transform.position.z + (rotationAxisY * warpBallSpawnDistance);
        transform.position = new Vector3(posX, posY, posZ);
    }

	void SetInitTrajectory(){
        float trajectoryAngle = creator.transform.Find("FPS Camera").rotation.eulerAngles.x;
        if (trajectoryAngle <= 90f)
            trajectoryAngle *= -1;
        else if (trajectoryAngle >= 270f)
            trajectoryAngle = 90f - (trajectoryAngle % 270f);

        float horizontalForceX = Mathf.Cos(trajectoryAngle * Mathf.Deg2Rad) * creator.transform.forward.x;
        float horizontalForceZ = Mathf.Cos(trajectoryAngle * Mathf.Deg2Rad) * creator.transform.forward.z;
        float verticalForce    = Mathf.Sin(trajectoryAngle * Mathf.Deg2Rad);
        Vector3 trajectoryVector3 = new Vector3(horizontalForceX, verticalForce, horizontalForceZ);

        rb.velocity = trajectoryVector3.normalized * warpBallInitSpeed;
	}

	void SetCreatorAsParent(){
	    transform.SetParent(creator.transform.Find("WarpBallPool").transform);
	}

	void SetGlobalWarpBallPoolAsParent(){
        transform.SetParent(GameObject.FindWithTag("GlobalWarpBallPool").transform);
    }

	public void SetActive(bool active){
        gameObject.SetActive(active);
    }

    public bool isActive(){
        return gameObject.activeSelf;
    }

    public Transform getTransform(){
        return transform;
    }

    public Vector3 getvelocity(){
        return rb.velocity;
    }
}
