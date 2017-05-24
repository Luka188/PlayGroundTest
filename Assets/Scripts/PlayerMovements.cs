using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMovements : MonoBehaviour
{
    //ints
    public int WallJumpSlope;
    //floats
    float maxSlope = 50;
    float jump = 0;
    float defaultFieldOfView;
    float SpaceCounter = 0;
    float oldVertical;
    float oldHorizontal;
    float currentV;
    float currentH;
    public float speed;
    public float MouseSpeed;
    public float JumpForce;
    public float mySpeed = 5;
    public float AirAcceleration = 2;
    public float GroundAcceleration = 4;
    //bools
    bool grounded = false;
    bool countingSpace;
    bool JumpWhenPossible = false;
    //Unity Stuffs
    Rigidbody myRigidBody;
    public Camera cam;
    public AnimationCurve curve;
    [SerializeField]
    Text DisplaySpeed;
    [SerializeField]
    RawImage WillJump;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        defaultFieldOfView = cam.fieldOfView;
        
    }
    void FixedUpdate()
    {


        float vertical = MyGetAxis(true);
        float horizontal = MyGetAxis(false);
        CheckJump();

        mySpeed = 10;
        Vector3 desiredSpeed = transform.forward * vertical * mySpeed + transform.right * horizontal * mySpeed;
        desiredSpeed = Vector3.ClampMagnitude(desiredSpeed, mySpeed);
        Vector3 toAdd;
        if (grounded)
            toAdd = new Vector3((desiredSpeed.x - myRigidBody.velocity.x) , jump + JumpFormula(), (desiredSpeed.z - myRigidBody.velocity.z));
        else
            toAdd = new Vector3((desiredSpeed.x - myRigidBody.velocity.x) , jump + JumpFormula(), (desiredSpeed.z - myRigidBody.velocity.z) );
       
        myRigidBody.AddForce(toAdd, ForceMode.VelocityChange);
        
        float currentSpeed = Pythagore(myRigidBody.velocity.x, myRigidBody.velocity.z);
        DisplaySpeed.text = currentSpeed.ToString("#.##");

        cam.fieldOfView = defaultFieldOfView + currentSpeed;
        jump = 0.0f;
        
    }
    /*
    void Mouselook()
    {
        float rotLeftRight = Input.GetAxis("Mouse X") * MouseSpeed;
        float rotUpDown = Input.GetAxis("Mouse Y") * MouseSpeed;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y+rotLeftRight);
        cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x-rotUpDown,cam.transform.eulerAngles.y);
        
    }*/
    /*
    float GetNewVertical(float vertical)
    {
        
        vertical =  Mathf.MoveTowards(lastVertical, vertical, Time.deltaTime *(vertical==0?AirControl/2:AirControl));
        lastVertical = vertical;
        return vertical;
    }*/
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
            currentV = Mathf.MoveTowards(currentV, current, Time.deltaTime*GetAcceleration());
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
            currentH = Mathf.MoveTowards(currentH, current, Time.deltaTime*GetAcceleration());
            return currentH;
        }
    }
    float GetAcceleration()
    {
        if (grounded)
            return GroundAcceleration;
        else
            return AirAcceleration;
    }
    float Pythagore(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }
    float JumpFormula()
    {
        if (SpaceCounter == 0)
            return 0;
        else
        {
            //print(SpaceCounter);
            return (8 / (1 + SpaceCounter))*Time.deltaTime;
        }
    }
    
    public void ResetVelocity()
    {
        currentH = 0;
        currentV = 0;
    }
    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            JumpWhenPossible = true;
            WillJump.color = Color.green;
        }
        if(JumpWhenPossible&&grounded)
        {
            //sCheckSprint();
            JumpWhenPossible = false;
            WillJump.color = Color.white;
            jump = JumpForce;
            grounded = false;
            countingSpace = true;
        }
        if (countingSpace)
        {
            SpaceCounter += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            JumpWhenPossible = false;
            WillJump.color = Color.white;
            countingSpace = false;
            SpaceCounter = 0;
        }
    }
    
    void OnCollisionStay(Collision coll)
    {
        //Debug.DrawRay(coll.contacts[0].point, transform.up, Color.red, 4);
        float angle = Vector3.Angle(coll.contacts[0].normal, Vector3.up);
        if (angle < maxSlope)
        {
            if (!grounded)
                grounded = true;
        }
        else if (angle < 90 + WallJumpSlope && angle > 90 - WallJumpSlope)
        {
            
            if (JumpWhenPossible)
            {
                JumpWhenPossible = false;
                WillJump.color = Color.white;
                countingSpace = true;
                Vector3 desiredJump = new Vector3(0, 2.5f-myRigidBody.velocity.y, 0);
                myRigidBody.AddForce(desiredJump, ForceMode.VelocityChange);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.contacts.Length == 0 && grounded)
            grounded = false;
    }






}