using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    // Maintained by: PLAYER ROTATION SYSTEM
    private Transform cameraTransform;      // PLAYER ROTATION SYSTEM Exclusive
    private float rotationAxisX;            // Used by: PLAYER INPUT SYSTEM & SHOOTING SYSTEM
    private float rotationAxisY;            // Used by: PLAYER INPUT SYSTEM & SHOOTING SYSTEM

    // Maintained by: PLAYER INPUT SYSTEM
    private Vector3 inputRotation;          // Used by: BODY MOVEMENT SYSTEM
    private bool jumpTriggered;             // Used by: BODY MOVEMENT SYSTEM
    private bool gunTriggered;              // Used by: SHOOTING SYSTEM
    private bool warpTriggered;             // Used by: WARPING SYSTEM

    // Maintained by: BODY MOVEMENT SYSTEM
    public float runSpeedMax;               // BODY MOVEMENT SYSTEM Exclusive
    public float runSpeedAcceleration;      // BODY MOVEMENT SYSTEM Exclusive
    public float runSpeedDeceleration;      // BODY MOVEMENT SYSTEM Exclusive
    public float distToGround;              // BODY MOVEMENT SYSTEM Exclusive
    public float jumpForce;                 // BODY MOVEMENT SYSTEM Exclusive

    private Rigidbody rb;                   // BODY MOVEMENT SYSTEM Exclusive
    public bool isGrounded;                 // BODY MOVEMENT SYSTEM Exclusive

    // Maintained by: SHOOTING SYSTEM
    public int maxWarpBalls;                        // SHOOTING SYSTEM Exclusive
    public WarpBallController warpBallController;   // SHOOTING SYSTEM Exclusive
    public WarpBallController nextWarpBallToFire;   // SHOOTING SYSTEM Exclusive
    public WarpBallController lastWarpBallToFire;   // SHOOTING SYSTEM Exclusive

    private List<WarpBallController> warpBalls;     // SHOOTING SYSTEM Exclusive
    private bool availableWarpBall;                 // SHOOTING SYSTEM Exclusive

    // Maintained by: WARPING SYSTEM
    // NONE ATM.
    public float currentSpeed;

    void Start(){
        InitiatePlayerRotationSystem();
        InitiatePlayerInputSystem();
        InitiateBodyMovementSystem();
        InitiateShootingSystem();
        InitiateWarpingSystem();
    }

    void Update() {
        HandlePlayerRotationSystem();
        HandlePlayerInputSystem();
        HandleShootingSystem();
        HandleWarpingSystem();
    }

    void FixedUpdate() {
        HandleBodyMovementSystem();
        
    }


    //----------------------------------------------------------------------------------
    //------------------------- PLAYER ROTATION SYSTEM: --------------------------------
    //----------------------------------------------------------------------------------

    //------------------------------------------------------
    //---      PLAYER ROTATION SYSTEM: INITIATION        ---
    //------------------------------------------------------
    void InitiatePlayerRotationSystem(){
        cameraTransform = transform.Find("FPS Camera");
        rotationAxisX = 0f;
        rotationAxisY = 0f;
    }

    //------------------------------------------------------
    //---       PLAYER ROTATION SYSTEM: UPDATE           ---
    //------------------------------------------------------
    void HandlePlayerRotationSystem(){
        RotatePlayerToCameraRotation();
        UpdateRotationVector();
    }

    void RotatePlayerToCameraRotation(){
        transform.rotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);
    }

    void UpdateRotationVector(){
        float frontAngle = transform.rotation.eulerAngles.y;
        if(frontAngle > 180) frontAngle = -180f + (frontAngle % 180f);

        rotationAxisX = Mathf.Sin(frontAngle * Mathf.Deg2Rad);
        rotationAxisY = Mathf.Cos(frontAngle * Mathf.Deg2Rad);
    }


    //----------------------------------------------------------------------------------
    //--------------------------- PLAYER INPUT SYSTEM: ---------------------------------
    //----------------------------------------------------------------------------------

    //------------------------------------------------------
    //---        PLAYER INPUT SYSTEM: INITIATION         ---
    //------------------------------------------------------
    void InitiatePlayerInputSystem(){
        inputRotation = new Vector3(0,0,0);
        jumpTriggered = false;
        gunTriggered = false;
        warpTriggered = false;
    }

    //------------------------------------------------------
    //---         PLAYER INPUT SYSTEM: UPDATE            ---
    //------------------------------------------------------
    void HandlePlayerInputSystem(){
        WasdInputMapper();
        PollGunTrigger();
        PollWarpTrigger();
        PollJumpTrigger();
    }

    void WasdInputMapper(){
        Vector3 wasdSum = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.A))
            wasdSum += new Vector3(-rotationAxisY, 0, rotationAxisX);
        if (Input.GetKey(KeyCode.D))
            wasdSum += new Vector3(rotationAxisY, 0, -rotationAxisX);
        if (Input.GetKey(KeyCode.W))
            wasdSum += new Vector3(rotationAxisX, 0, rotationAxisY);
        if (Input.GetKey(KeyCode.S))
            wasdSum += new Vector3(-rotationAxisX, 0, -rotationAxisY);

        inputRotation = Vector3.Normalize(wasdSum);
    }

    void PollGunTrigger(){
        gunTriggered = Input.GetMouseButtonDown(0);
    }

    void PollWarpTrigger(){
        warpTriggered = Input.GetMouseButtonDown(1);
        //Debug.Log("warpTriggered :" + warpTriggered);
    }

    void PollJumpTrigger(){
        if (Input.GetKeyDown(KeyCode.Space)){
            jumpTriggered = true;
        }

        if (Input.GetKeyUp(KeyCode.Space)){
            jumpTriggered = false;
        }
    }


    //----------------------------------------------------------------------------------
    //-------------------------- BODY MOVEMENT SYSTEM: ---------------------------------
    //----------------------------------------------------------------------------------

    //------------------------------------------------------
    //---       BODY MOVEMENT SYSTEM: INITIATION         ---
    //------------------------------------------------------
    void InitiateBodyMovementSystem(){
        rb = GetComponent<Rigidbody>();
        isGrounded = true;

    }

    //------------------------------------------------------
    //---         BODY MOVEMENT SYSTEM: UPDATE           ---
    //------------------------------------------------------
    void HandleBodyMovementSystem(){
        if(isGrounded)
            Run();
        Jump();
        PollLanding();
        currentSpeed = rb.velocity.magnitude;
    }

    void Run(){
        // TODO: Kan hende acceleration hadde vært bedre.

        float newRunSpeedX = rb.velocity.x;
        float newRunSpeedZ = rb.velocity.z;

        if(inputRotation.magnitude != 0){
            if (rb.velocity.magnitude < runSpeedMax * 0.9){
                float allowedSpeedChangeX = inputRotation.x * runSpeedMax - rb.velocity.x;
                float allowedSpeedChangeZ = inputRotation.z * runSpeedMax - rb.velocity.y;
                newRunSpeedX += (allowedSpeedChangeX * runSpeedAcceleration) / runSpeedMax;
                newRunSpeedZ += (allowedSpeedChangeZ * runSpeedAcceleration) / runSpeedMax;
            } else {
                newRunSpeedX = inputRotation.x * runSpeedMax;
                newRunSpeedZ = inputRotation.z * runSpeedMax;
            }
        } else {
            if (rb.velocity.magnitude > runSpeedDeceleration){
                float allowedSpeedChangeX = rb.velocity.x;
                float allowedSpeedChangeZ = rb.velocity.z;

                newRunSpeedX -= (allowedSpeedChangeX / runSpeedMax) * runSpeedDeceleration;
                newRunSpeedZ -= (allowedSpeedChangeZ / runSpeedMax) * runSpeedDeceleration;
            } else {
                newRunSpeedX = 0f;
                newRunSpeedZ = 0f;
            }
        }

        rb.velocity = new Vector3(newRunSpeedX, rb.velocity.y, newRunSpeedZ);
    }

    void Jump(){
        // TODO
        if (jumpTriggered && isGrounded){
            Vector3 addedJumpForce = new Vector3(0, jumpForce, 0);
            rb.velocity += addedJumpForce;
        }
    }

    void PollLanding(){
        // TODO
        isGrounded = IsGrounded();
    }

    bool IsGrounded() {
        GameObject body = transform.Find("Body").gameObject;
        GameObject cylinder = body.transform.Find("Cylinder").gameObject;
        float DisstanceToTheGround = cylinder.GetComponent<CapsuleCollider>().bounds.extents.y;

        return Physics.Raycast(transform.position, Vector3.down, DisstanceToTheGround + 0.1f);
    }


    //----------------------------------------------------------------------------------
    //---------------------------- SHOOTING SYSTEM: ------------------------------------
    //----------------------------------------------------------------------------------

    //------------------------------------------------------
    //---          SHOOTING SYSTEM: INITIATION           ---
    //------------------------------------------------------
    void InitiateShootingSystem(){
        warpBalls = new List<WarpBallController>();
        availableWarpBall = true;
        maxWarpBalls = 3;
        InstantiateInactiveWarpBalls();
    }

    void InstantiateInactiveWarpBalls(){
        Assert.IsTrue(maxWarpBalls > 0);
        while (warpBalls.Count < maxWarpBalls){
            WarpBallController warpBall = InstantiateInactiveWarpBall();
            warpBalls.Add(warpBall);
        }
        nextWarpBallToFire = warpBalls[0];
    }

    WarpBallController InstantiateInactiveWarpBall(){
        WarpBallController warpBall;
        warpBall = Instantiate(warpBallController,
                               new Vector3(0, 0, 0),
                               transform.rotation) as WarpBallController;
        warpBall.InitiateByCreator(this.gameObject, rotationAxisX, rotationAxisY);
        warpBall.SetActive(false);
        return warpBall;
    }

    //------------------------------------------------------
    //---            SHOOTING SYSTEM: UPDATE             ---
    //------------------------------------------------------
    void HandleShootingSystem(){
        findAvailableWarpBall();
        if (shouldGunFire())
            LaunchWarpBall();
        PollWarpBallPickUp();
    }

    void findAvailableWarpBall(){
        availableWarpBall = false;
        foreach(WarpBallController warpBall in warpBalls){
            if (!warpBall.isActive()){
                nextWarpBallToFire = warpBall;
                availableWarpBall = true;
                return;
            }
        }
    }

    bool shouldGunFire(){
        return gunTriggered && isPlayerAbleToShoot();
    }

    bool isPlayerAbleToShoot(){
        return availableWarpBall;
    }

    void LaunchWarpBall (){
        nextWarpBallToFire.LaunchFromCreator(rotationAxisX, rotationAxisY);
        lastWarpBallToFire = nextWarpBallToFire;
    }

    void PollWarpBallPickUp(){
        // TODO REWORK
        if (Input.GetKey(KeyCode.T)){
            if (warpBalls.Count >= maxWarpBalls){
                warpBalls[0].SetActive(false);
            }
        }
    }

    //----------------------------------------------------------------------------------
    //----------------------------- WARPING SYSTEM: ------------------------------------
    //----------------------------------------------------------------------------------

    //------------------------------------------------------
    //---           WARPING SYSTEM: INITIATION           ---
    //------------------------------------------------------
    void InitiateWarpingSystem(){
    }

    //------------------------------------------------------
    //---             WARPING SYSTEM: UPDATE             ---
    //------------------------------------------------------
    void HandleWarpingSystem(){
        //canWarp
        //warpTriggered
        //lastWarpBallToFire


        if(ShouldPlayerWarp()){
            InheritWarpBallValues();
            PickUpWarpBall();
            WarpPlayer();
        }
    }

    bool ShouldPlayerWarp(){
        // TODO: Legg til test for om spilleren har plass dit den skal.
        if (lastWarpBallToFire != null)
            return lastWarpBallToFire.isActive() && warpTriggered;
        return false;
    }

    void InheritWarpBallValues(){
        // TODO: sett rotasjon, velocity til spilleren lik ballen.

        transform.rotation = lastWarpBallToFire.getTransform().rotation;
        rb.velocity = lastWarpBallToFire.getvelocity();
    }

    void PickUpWarpBall(){
        // TODO: Deaktiver ballen.
        // TODO: REWORK
        warpBalls[0].SetActive(false);
    }

    void WarpPlayer(){
        // TODO: Sett posisjonen til spilleren der ballen lå.
        transform.position = lastWarpBallToFire.getTransform().position;
    }
}
