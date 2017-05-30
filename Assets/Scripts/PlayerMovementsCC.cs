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
    public float WallJumpingForce;

    float myTimeDelta;
    float currentH;
    float currentV;
    float currentJ = 1;
    float currentWJ = 0;
    float spaceCounter;
    float timeInAir = 0;
    float VelJumpAddCounter = 0;
    float CurrentSlide = 0;

    //bools
    bool willJump;
    bool countingSpace;
    bool canWallJump;
    bool willSlide;
    bool applyGravity = true;

    //Unity Stuffs
    public AnimationCurve WallCurve;
    public AnimationCurve FallinCurve;
    public AnimationCurve VelJump;
    [SerializeField]
    RawImage JumpingVisual;
    [SerializeField]
    Text DisplaySpeed;

    Transform myTransform;
    CharacterController CC;
    Vector3 lastWall;
    Vector3 redNormal;

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
        CheckSlide();
        Vector3 Acceleration = (myTransform.forward * vertical * speed + myTransform.right * horizontal * speed);
        Acceleration = Vector3.ClampMagnitude(Acceleration, speed);
        applyGravity = true;
        if (MovementState.WallJumping)
        {
            if (NeedCorrection(Acceleration))
            {
                Vector3 Correction = Vector2.Dot(new Vector2(redNormal.z,redNormal.x), Acceleration) * new Vector2(redNormal.z, redNormal.x)*WallCurve.Evaluate(timeInAir);
                Vector3 NewAcc= Acceleration*(1-WallCurve.Evaluate(timeInAir));
                Acceleration = new Vector3(Correction.x + NewAcc.x, Correction.y + NewAcc.y, Correction.z + NewAcc.z);
                Debug.DrawLine(transform.position, transform.position + Acceleration, vertical==1&&horizontal==1? Color.red:Color.green, 10);
            }
        }
        else if (MovementState.Sliding)
        {
            if (CheckStillOnWall())
            {
                Acceleration = (myTransform.forward * speed );
                Acceleration = Vector2.Dot(new Vector2(redNormal.z, redNormal.x), Acceleration)* new Vector3(redNormal.z, 0, -redNormal.x) * speed; ;
                applyGravity = false;
            }
        }
        else if (MovementState.GroundSliding)
        {
            Acceleration = Acceleration * VelJump.Evaluate(CurrentSlide / 2);
        }
        if (MovementState.VelJumping)
        {
            Vector3 prev = Vector3.ClampMagnitude(myTransform.forward * vertical + myTransform.right * horizontal,1) ;
            Acceleration += prev * VelJump.Evaluate(VelJumpAddCounter) * (VelocityAddition);
        }
        Acceleration = Vector3.ClampMagnitude(Acceleration, speed + (MovementState.VelJumping ? VelJump.Evaluate(VelJumpAddCounter) * (VelocityAddition):0)) *myTimeDelta;
        if (MovementState.WallJumping&&!MovementState.Sliding)
            Acceleration +=  lastWall* WallCurve.Evaluate(currentWJ) * WallJumpingForce*myTimeDelta;
        if(applyGravity)
            Acceleration += (myTransform.up * Physics.gravity.y * FallinCurve.Evaluate(timeInAir) + myTransform.up * currentJ + JumpFormula()*myTransform.up) * myTimeDelta;
        CC.Move(Acceleration);
        UpdateVelJump();
        UpdateJump();
        float p = Pythagore(Acceleration.x / myTimeDelta, Acceleration.z / myTimeDelta);
        DisplaySpeed.text = p.ToString("#.##");
    }
    bool NeedCorrection(Vector3 Acc)
    {
        Vector2 minus = new Vector2(Acc.x - redNormal.x, Acc.z - redNormal.z);
        Vector2 add = new Vector2(Acc.x + redNormal.x, Acc.z + redNormal.z);
        return minus.magnitude > add.magnitude;
    }
    float Pythagore(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
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
            
            currentV = Mathf.MoveTowards(currentV, current, myTimeDelta * GetAcceleration() * (current == 0 ? 0.5f : 1f));
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
            currentH = Mathf.MoveTowards(currentH, current, myTimeDelta * GetAcceleration() * (current == 0 ? 0.5f : 1f));
            return currentH;
        }
    }
    void CheckSlide()
    {
        if (Input.GetKey(KeyCode.LeftShift)&&!MovementState.Sliding&&!MovementState.GroundSliding)
        {
            willSlide = true;
            speed = 13f;
            CurrentSlide = 0;
        }
        if (willSlide && CC.isGrounded)
        {
            CC.height = 0;
            willSlide = false;
            MovementState.GroundSliding = true;
        }
        if (MovementState.GroundSliding)
        {
            CurrentSlide += myTimeDelta;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)&&MovementState.GroundSliding)
        {
            MovementState.GroundSliding = false;
            willSlide = false;
            MovementState.Sliding = false;
            CC.height = 1;
            speed = 10f;
            CurrentSlide = 0;
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
    bool CheckStillOnWall()
    {
        RaycastHit hitRay;
        Debug.DrawLine(transform.position, transform.position -redNormal*4, Color.red);
        if (Physics.Raycast(myTransform.position, -redNormal, out hitRay,2))
        {
            MovementState.Sliding = true;
            redNormal = hitRay.normal;
            CheckWallJump(hitRay.normal, hitRay.point);
            return true;
        }
        else
        {
            MovementState.Sliding = false;
            speed = 10f;
            return false;
        }
        
    }
    void Jump()
    {
        JumpingVisual.color = Color.white;
        currentJ = jumpHeight;
        willJump = false;
        countingSpace = true;
        timeInAir = 0;
        MovementState.VelJumping = true;
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
        if (currentJ > 1)
            currentJ -= myTimeDelta*jumpHeight;
        else
            currentJ = 1;
        if (MovementState.WallJumping)
        {
            currentWJ += myTimeDelta;
            if (CC.isGrounded || currentWJ > 1)
            {
                currentWJ = 0;
                MovementState.WallJumping = false;
            }
        }
    }
    void UpdateVelJump()
    {
        if (MovementState.VelJumping)
        {
            VelJumpAddCounter += myTimeDelta;
            if (VelJumpAddCounter > 1)
            {
                MovementState.VelJumping = false;
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
    bool CheckWallJump(Vector3 normal, Vector3 Point)
    {
        if (willJump)
        {
            currentWJ = 0;
            MovementState.WallJumping = true;
            lastWall = -new Vector3(Point.x - myTransform.position.x, 0, Point.z - myTransform.position.z);
            redNormal = normal;
            Debug.DrawLine(Point, Point + normal, Color.red, 2);
            Jump();
            currentJ = 13;
            return true;
        }
        else
            return false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(Mathf.Abs(hit.normal.x)+Mathf.Abs(hit.normal.z)>SlopeToWallJump)
        {
            if (CheckWallJump(hit.normal, hit.point))
            {
                
            }
            else if (willSlide)
            {
                willSlide = false;
                print("Sliding");
                MovementState.Sliding = true;
                redNormal = hit.normal;
                MovementState.VelJumping = true;
            }
        }
    }




}
