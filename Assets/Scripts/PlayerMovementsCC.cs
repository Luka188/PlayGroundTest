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
    public float SledgingSpeed;
    public float speedToAddEachJump;
    public float speedLosingGround;
    public float InvincibiltyAfterJump;

    float myTimeDelta;
    float currentH;
    float currentV;
    float currentJ = 1;
    float currentWJ = 0;
    float spaceCounter;
    float additionalSpeed = 0;
    float timeInAir = 0;
    float CurrentSlide = 0;
    float currentSledging = 0;
    float RabbitCounter;
    float calculatedSpeed;
    float currentInvincibility;
   
    //bools
    bool willJump;
    bool countingSpace;
    bool canWallJump;
    bool willSlide;
    bool applyGravity = true;
    bool willGetBoost = false;
    bool wannaSpeedUp = false;

    //Unity Stuffs
    public AnimationCurve WallCurve;
    public AnimationCurve FallinCurve;
    public AnimationCurve RabbitCurve;
    [SerializeField]
    RawImage JumpingVisual;
    [SerializeField]
    Text DisplaySpeed;

    Transform myTransform;
    CharacterController CC;
    Vector3 redNormal;
    Vector3 glissadeDir;
    Camera cam;
    [SerializeField]
    ParticleSystem SpeedParts;
    [SerializeField]
    ParticleSystem WallSparkles;
    [SerializeField]
    ForceRotation ForceRot;
    [SerializeField]
    UIAnimation UIanim;
   

    GameObject LastWall;
    GameObject LastGreen;
    public void Reset()
    {
        LastWall = null;
        MovementState.GroundSliding = false;
        willJump = false;
        willSlide = false;
        countingSpace = false;
        additionalSpeed = 0;
        timeInAir = 0;
        currentH = 0;
        currentV = 0;
        currentJ = 1;
        RabbitCounter = 3;
        currentWJ = 0;
    }
    private void Start()
    {
        myTransform = transform;
        CC = myTransform.GetComponent<CharacterController>();
        cam = myTransform.GetChild(0).GetComponent<Camera>();
   
    }
    // Update is called once per frame
    void Update()
    {
        myTimeDelta = Time.deltaTime * (1 / Time.timeScale);
        float vertical = MyGetAxis(true);
        float horizontal = MyGetAxis(false);
        CheckJump();
        //CheckLeftClick();
        CheckSlide();
        CheckWannaSpeed();
        Vector3 Acceleration = (myTransform.forward * vertical  + myTransform.right * horizontal );
        Acceleration = Vector3.ClampMagnitude(Acceleration, 1);
        applyGravity = true;
        if (MovementState.WallJumping)
        {

        }

        else if (MovementState.GroundSliding)
        {
            Acceleration = (glissadeDir);
        }

        Acceleration = Acceleration * (speed+ additionalSpeed)*myTimeDelta;
        if (MovementState.Sledging)
        {
            Acceleration = (glissadeDir * vertical * speed);
            Acceleration *= FallinCurve.Evaluate(currentSledging / 4) * SledgingSpeed;
        }
        if(applyGravity)
            Acceleration += (myTransform.up * Physics.gravity.y * FallinCurve.Evaluate(timeInAir) + myTransform.up * currentJ + JumpFormula()*myTransform.up) * myTimeDelta;
        Vector3 p1 = myTransform.position;
        CC.Move(Acceleration);
        Vector3 p2 = myTransform.position;
        Vector3 p3 =(p1 - p2) / myTimeDelta;
        calculatedSpeed = Pythagore(p3.x, p3.z);
        UpdateSpeed();
        CheckGrounded();
        SmoothRabbitJump();
        UpdateJump();
        UpdateInvincibilty();
        float p = Pythagore(Acceleration.x / myTimeDelta, Acceleration.z / myTimeDelta);
        cam.fieldOfView = 60 + additionalSpeed;
        CheckSpeedParticles(Acceleration);
        DisplaySpeed.text = calculatedSpeed.ToString("#.##");
    }
    void CheckGrounded()
    {
        if(CC.isGrounded&&LastWall!=null)
        {
            LastWall = null;
        }
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
    float Pythagore3D(float x, float y, float z)
    {
        return Mathf.Sqrt(x * x + y * y + z * z);
    }
    void CheckSpeedParticles(Vector3 acc)
    {
        float vitesse;
        acc /= myTimeDelta;
        if (timeInAir > 0)
            vitesse = Pythagore3D(acc.x , acc.y , acc.z );
        else
            vitesse = Pythagore(acc.x, acc.z);
        if (vitesse > 14)
        {
            
            var k = SpeedParts.emission;
            k.rateOverTime = (vitesse - 10)+2;
            k.enabled = true;
            
        }
        else if (SpeedParts.emission.enabled)
        {
            var k = SpeedParts.emission;
            k.enabled = false;

        }
            
    }
    void UpdateInvincibilty()
    {
        currentInvincibility -= myTimeDelta;
    }
    void CheckSlide()
    {
        if (Input.GetKey(KeyCode.F)&&!MovementState.Sliding&&!MovementState.GroundSliding&&CurrentSlide==0)
        {
            willSlide = true;
            
        }
        if (willSlide && CC.isGrounded)
        {
            CC.height = 0;
            willSlide = false;
            MovementState.GroundSliding = true;
            glissadeDir = transform.forward;
            
        }
        if (MovementState.GroundSliding)
        {
            CurrentSlide += myTimeDelta;
        }
        if (CurrentSlide>1)
        {
            CC.height = (CurrentSlide-1) * 5;
            RabbitCounter = 2;
            if (CC.height > 1)
            {
                CC.height = 1;
                MovementState.GroundSliding = false;
                willSlide = false;
                MovementState.Sliding = false;
                CC.height = 1;
                CurrentSlide = 0;
            }

        }
        if (MovementState.Sledging)
        {
            currentSledging += myTimeDelta;
        }
        else if(currentSledging>0)
        {
            currentSledging -= myTimeDelta * 3;
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
            if (MovementState.GroundSliding)
            {
                MovementState.GroundSliding = false;
                CC.height = 1;
                CurrentSlide = 0;
                willSlide = false;
            }
            
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
    void CheckWannaSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            wannaSpeedUp = true;
        }
        else
            wannaSpeedUp = false;
    }
    bool CheckStillOnWall()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            RaycastHit hitRay;
            Debug.DrawLine(transform.position, transform.position - redNormal * 4, Color.red);
            if (Physics.Raycast(myTransform.position, -redNormal, out hitRay, 2))
            {
                MovementState.Sliding = true;
                redNormal = hitRay.normal;
               
                return true;
            }
            else
            {
                MovementState.Sliding = false;
                WallSparkles.Stop();
                return false;
            }
        }
        WallSparkles.Stop();
        MovementState.Sliding = false;
        return false;
        
    }
    void Jump()
    {
        if (wannaSpeedUp)
        {
            additionalSpeed += speedToAddEachJump;
            UIanim.AnimateGreen();
        }
        JumpingVisual.color = Color.white;
        currentJ = jumpHeight;
        willJump = false;
        countingSpace = true;
        timeInAir = 0;
        currentInvincibility = InvincibiltyAfterJump;
        RabbitCounter = 0;
        cam.transform.localPosition = Vector3.up * 0.5f;
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
    void UpdateSpeed()
    {
        if (calculatedSpeed < speed + additionalSpeed - 10&&currentInvincibility<=0)
        {
            additionalSpeed -= additionalSpeed -= myTimeDelta * speedLosingGround * (speed + additionalSpeed);
        }
        if (CC.isGrounded&&!MovementState.GroundSliding&&!willJump)
        {
            if (additionalSpeed > 0)
                additionalSpeed -= myTimeDelta * speedLosingGround * (speed + additionalSpeed);
            else
                additionalSpeed = 0;
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
    void SmoothRabbitJump()
    {
        if (CC.isGrounded && !MovementState.GroundSliding)
        {
            RabbitCounter += myTimeDelta * 8;
            if (RabbitCounter < 2)
            {
                cam.transform.localPosition = Vector3.up * RabbitCurve.Evaluate(RabbitCounter);

            }
            else cam.transform.localPosition = Vector3.up* 0.5f;
        }
        else if (CC.height != 1)
            cam.transform.localPosition = Vector3.up * 0.5f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Boost")
        {
            willGetBoost = true;
            LastGreen = hit.gameObject;
        }
        else
            willGetBoost = false;
        if (Mathf.Abs(hit.normal.x)+Mathf.Abs(hit.normal.z)>SlopeToWallJump)
        {
            if (willJump&&LastWall!= hit.gameObject)
            {
                
                Jump();
                LastWall = hit.gameObject;
            }
            
        }

    }




}
