using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour {

    public int numberOfPoints;
    public TrajectoryPointController trajectoryPointController;

    public float trajectoryDistance;
    public float warpBallInitSpeed = 20f;


    private List<TrajectoryPointController> trajectoryPoints;

    // Use this for initialization
    void Start () {
        trajectoryPoints = new List<TrajectoryPointController>();
        InstantiateTrajectoryPoints();
    }

    // Update is called once per frame
    void Update () {

    }

    void InstantiateTrajectoryPoints(){
        int index = 0;
        while (trajectoryPoints.Count < numberOfPoints){
            TrajectoryPointController trajectoryPoint = InstantiateTrajectoryPoint(index);
            trajectoryPoints.Add(trajectoryPoint);
            index += 1;
        }
    }

    TrajectoryPointController InstantiateTrajectoryPoint(int index){
        TrajectoryPointController trajectoryPoint;
        trajectoryPoint = Instantiate(trajectoryPointController,
                                      new Vector3(0, 0, 0),
                                      transform.rotation) as TrajectoryPointController;
        trajectoryPoint.InitiateByCreator(this.gameObject, index, trajectoryDistance, warpBallInitSpeed);
//        trajectoryPoint.SetActive(false);
        return trajectoryPoint;
    }
}
