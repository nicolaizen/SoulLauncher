using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPointController : MonoBehaviour {

	public Vector3 initialVelocity;
    public Vector3 initialPosition;

    public GameObject creator;
    public Transform orientation;

    private Vector3 gravity;
    private float timer;
    public int index;
    private float trajectoryDistance;
    private float warpBallInitSpeed;

    void FixedUpdate () {
        timer += Time.deltaTime;
        transform.position = CalculatePosition((float)index / 20);

        if (Input.GetKey(KeyCode.R))
            timer = 0;
    }

    Vector3 GetInitTrajectory(){
        float trajectoryAngle = orientation.rotation.eulerAngles.x;
        if (trajectoryAngle <= 90f)
            trajectoryAngle *= -1;
        else if (trajectoryAngle >= 270f)
            trajectoryAngle = 90f - (trajectoryAngle % 270f);

//        Debug.Log("trajectoryAngle: " + trajectoryAngle);
        Vector3 trajectoryVector3 = new Vector3(0f, trajectoryAngle, 90f - Mathf.Abs(trajectoryAngle));


        return trajectoryVector3.normalized * warpBallInitSpeed;
    }


    private Vector3 CalculatePosition(float elapsedTime){
        // p(t) = (g*t^2)/2 + v0*t + p0

        Vector3 new_pos = (gravity * elapsedTime * elapsedTime) +
                          (GetInitTrajectory() * elapsedTime) +
                          new Vector3(0f, 1f, 0f);
//        Debug.Log("new_pos: " + new_pos);
//        Debug.Log("elapsedTime: " + elapsedTime);
        Debug.Log("GetInitTrajectory: " + GetInitTrajectory());

        return transform.parent.transform.position + new_pos;
    }

    public void InitiateByCreator(GameObject creator, int index, float trajectoryDistance, float warpBallInitSpeed){
        timer = 0;
        gravity = Physics.gravity;
        this.index = index;
        this.creator = creator;
        this.trajectoryDistance = trajectoryDistance;
        this.warpBallInitSpeed = warpBallInitSpeed;
        transform.SetParent(creator.transform);
        orientation = transform.parent.parent.transform.Find("FPS Camera");
    }

    public void SetActive(bool active){
        gameObject.SetActive(active);
    }
}
