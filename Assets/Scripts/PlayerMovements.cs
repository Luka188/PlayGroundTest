using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMovements : MonoBehaviour
{
    //ints
    
    //floats
    float maxSlope = 50;
    float jump = 0;
    float defaultFieldOfView;
    float speedToAdd;
    float SpaceCounter = 0;
    float lastVertical = 0;
    public float speed;
    public float MouseSpeed;
    public float JumpForce;
    public float mySpeed = 5;
    public float AirControl = 2;
   
    //bools
    bool grounded = false;
    bool Sprinting = false;
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
    void Update()
    {
        

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        CheckJump();
        CheckSprint();
        mySpeed = CalculateSpeed();
        Vector3 desiredSpeed;
        Vector3 toAdd;
        if (Sprinting)
        {
            if (!grounded)
                desiredSpeed = transform.forward * GetNewVertical(vertical) * mySpeed;
            else
                desiredSpeed = transform.forward * vertical * mySpeed;
           toAdd = new Vector3(desiredSpeed.x - myRigidBody.velocity.x, jump + JumpFormula(), desiredSpeed.z - myRigidBody.velocity.z);
            myRigidBody.AddForce(toAdd, ForceMode.VelocityChange);
        }
        else
        {
            desiredSpeed = (transform.forward * vertical * mySpeed + transform.right * horizontal * mySpeed);
            if (!grounded)
            {
                 
                toAdd = new Vector3((desiredSpeed.x - myRigidBody.velocity.x) * Time.deltaTime * AirControl, jump + JumpFormula(), (desiredSpeed.z - myRigidBody.velocity.z) * Time.deltaTime * AirControl);
            }
            else
                toAdd = new Vector3(desiredSpeed.x - myRigidBody.velocity.x, jump + JumpFormula(), desiredSpeed.z - myRigidBody.velocity.z);
            myRigidBody.AddForce(toAdd, ForceMode.VelocityChange);
        }
        float currentSpeed = Pythagore(myRigidBody.velocity.x, myRigidBody.velocity.z);
        DisplaySpeed.text = currentSpeed.ToString("#.##");

        cam.fieldOfView = defaultFieldOfView + (mySpeed>5?mySpeed-5:0) * 2;
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
    float GetNewVertical(float vertical)
    {
        if (vertical < lastVertical)
        {
            vertical = lastVertical- Time.deltaTime*AirControl/4;
        }
        lastVertical = vertical;
        return vertical;
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
            //print((1 / (1 + SpaceCounter)));
            //print(SpaceCounter);
            return (1 / 2 * (1 + SpaceCounter));
        }
    }
    float CalculateSpeed()
    {
        float TmpSpeed = mySpeed + speedToAdd * Time.deltaTime * 4;
        if (TmpSpeed < 5)
            return 5;
        else if (TmpSpeed > 10)
            return 10;
        return TmpSpeed;
    }
    void CheckSprint()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speedToAdd = -5;
            Sprinting = false;
        }
        if (Input.GetKey(KeyCode.LeftShift)&&grounded)
        {
            Sprinting = true;
            speedToAdd = 5;
        }
        
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            JumpWhenPossible = true;
            WillJump.color = Color.green;
        }
        if(JumpWhenPossible&&grounded)
        {
            CheckSprint();
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
        }
    }
    
    void OnCollisionStay(Collision coll)
    {
        //Debug.DrawRay(coll.contacts[0].point, transform.up, Color.red, 4);
        if (Vector3.Angle(coll.contacts[0].normal, Vector3.up) < maxSlope)
        {
            
            if (!grounded)
            {
                grounded = true;
            }
            
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.contacts.Length == 0 && grounded)
            grounded = false;
    }






}