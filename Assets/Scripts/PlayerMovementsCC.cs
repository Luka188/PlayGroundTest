using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementsCC : MonoBehaviour
{
    

    //Floats
    public float speed;
    public float jumpHeight;
    public float airAcceleration;
    public float groundAcceleration;
    public float JumpPushedForce;
    public float SlopeToWallJump;
    public float VelocityAddition;

    float myTimeDelta;
    float currentH;
    float currentV;
    float currentJ = 1;
    float currentWJ = 0;
    float spaceCounter;
    float timeInAir = 0;
    float VelJumpAddCounter = 0;

    //bools
    bool willJump;
    bool countingSpace;
    bool canWallJump;
    bool wallJumping;
    bool velJumping;

    //Unity Stuffs
    public AnimationCurve WallCurve;
    public AnimationCurve FallinCurve;
    public AnimationCurve VelJump;
    [SerializeField]
    RawImage JumpingVisual;

    Transform myTransform;
    CharacterController CC;
    Vector3 lastWall;
    Vector3 redVector;


    private void Start()
    {
        myTransform = transform;
        CC = myTransform.GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        myTimeDelta = Time.deltaTime * (1 / Time.timeScale);
        float vertical = MyGetAxis(true);
        float horizontal = MyGetAxis(false);
        CheckJump();
        CheckLeftClick();

        Vector3 Acceleration = (myTransform.forward * vertical * speed + myTransform.right * horizontal * speed);
        if(wallJumping)
        {
            

            if (GetAngle(redVector, Acceleration) > 0 && timeInAir < 0.7f) 
            {
                print(GetAngle(redVector, Acceleration));
                Acceleration = Vector2.Dot(redVector, Acceleration) * redVector;
            }
        }
        if(velJumping)
            Acceleration *= VelJump.Evaluate(VelJumpAddCounter)*(VelocityAddition/2);
        Acceleration = Vector3.ClampMagnitude(Acceleration, speed* VelJump.Evaluate(VelJumpAddCounter)*VelocityAddition)*myTimeDelta;
        if (wallJumping)
            Acceleration +=  lastWall* WallCurve.Evaluate(currentWJ) * speed * myTimeDelta;
        Acceleration += (myTransform.up * Physics.gravity.y * FallinCurve.Evaluate(timeInAir) + myTransform.up * currentJ + JumpFormula()*myTransform.up) * myTimeDelta;
        CC.Move(Acceleration);
        UpdateVelJump();
        UpdateJump();
    }
    float GetAngle(Vector3 V1, Vector3 V2)
    {
        float toreturn = Vector3.Angle(V1, V2);
        Vector3 cross = Vector3.Cross(V1, V2);
        return cross.y < 0 ? -toreturn : toreturn;
    }

    float MyGetAxis(bool Vertical)
    {
        if (Vertical)
        {
            float current;
            if (Input.GetKey(KeyCode.Z))
            {
                current = 1;
                if (Input.GetKey(KeyCode.S))
                    current = 0;
            }
            else if (Input.GetKey(KeyCode.S))
                current = -1;
            else current = 0;
            currentV = Mathf.MoveTowards(currentV, current, myTimeDelta * GetAcceleration());
            return currentV;
        }
        else
        {
            float current;
            if (Input.GetKey(KeyCode.D))
            {
                current = 1;
                if (Input.GetKey(KeyCode.Q))
                    current = 0;
            }
            else if (Input.GetKey(KeyCode.Q))
                current = -1;
            else current = 0;
            currentH = Mathf.MoveTowards(currentH, current, myTimeDelta * GetAcceleration());
            return currentH;
        }
    }
    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpingVisual.color = Color.green;
            willJump = true;
        }
        if (willJump&&CC.isGrounded)
        {
            Jump();
        }
        if (countingSpace)
        {
            spaceCounter += myTimeDelta;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            JumpingVisual.color = Color.white;
            willJump = false;
            countingSpace = false;
            spaceCounter = 0;
        }

    }
    void Jump()
    {
        JumpingVisual.color = Color.white;
        currentJ = jumpHeight;
        willJump = false;
        countingSpace = true;
        timeInAir = 0;
        velJumping = true;
        VelJumpAddCounter = 0;
    }
    float JumpFormula()
    {
        if (spaceCounter == 0)
            return 0;
        else
            return JumpPushedForce  / (1 + spaceCounter);
    }
    void UpdateJump()
    {
        if (CC.isGrounded && timeInAir > 0)
            timeInAir = 0;
        if (!CC.isGrounded)
            timeInAir += myTimeDelta;
       // print(FallinCurve.Evaluate(timeInAir));
        if (currentJ > 1)
            currentJ -= myTimeDelta*jumpHeight;
        else
            currentJ = 1;
        if (wallJumping)
        {
            currentWJ += myTimeDelta;
            if (CC.isGrounded || currentWJ > 1)
            {
                currentWJ = 0;
                wallJumping = false;
            }
        }
    }
    void UpdateVelJump()
    {
        if (velJumping)
        {
            VelJumpAddCounter += myTimeDelta;
            if (VelJumpAddCounter > 1)
            {
                velJumping = false;
                VelJumpAddCounter = 0;
            }
        }
    }
    float GetAcceleration()
    {
        if (CC.isGrounded)
            return groundAcceleration;
        else
            return airAcceleration;
    }
    void CheckLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TimeManager.ChangeTime(20);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            TimeManager.ResetTime();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
       
        if(Mathf.Abs(hit.normal.x)+Mathf.Abs(hit.normal.z)>SlopeToWallJump)
        {
            //WallJumping
            if (willJump)
            {
                currentWJ = 0;
                wallJumping = true;
                lastWall = -new Vector3(hit.point.x - myTransform.position.x,0, hit.point.z - myTransform.position.z);
                redVector = new Vector2(hit.normal.z, hit.normal.x);
                Jump();
            }
        }
    }




}
