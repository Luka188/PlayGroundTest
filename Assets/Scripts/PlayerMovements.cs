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
    float speedToDecrease = 5;
    float SpaceCounter = 0;
    public float speed;
    public float MouseSpeed;
    public float JumpForce;
    public float mySpeed = 5;
   
    //bools
    bool grounded = false;
    bool Sprinting = false;
    bool countingSpace;

    //Unity Stuffs
    Rigidbody myRigidBody;
    public Camera cam;
    public AnimationCurve curve;

    [SerializeField]
    Text DisplaySpeed;


    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        defaultFieldOfView = cam.fieldOfView;
        
    }
    void Update()
    {

        Mouselook();
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        CheckJump();
        CheckSprint();
        if (Sprinting)
        {
            mySpeed = mySpeed + Time.deltaTime* curve.Evaluate((mySpeed / 15f));
            Vector3 desiredSpeed = transform.forward * vertical * mySpeed ;
            Vector3 toadd = new Vector3(desiredSpeed.x - myRigidBody.velocity.x, jump + JumpFormula(), desiredSpeed.z - myRigidBody.velocity.z);
            myRigidBody.AddForce(toadd, ForceMode.VelocityChange);
        }
        else
        {
            mySpeed = mySpeed-(speedToDecrease-5)*Time.deltaTime*4;
            if (mySpeed < 5)
            {
                speedToDecrease = 5;
                mySpeed = 5;
            }
            Vector3 desiredSpeed = transform.forward * vertical * mySpeed  + transform.right * horizontal * mySpeed;
            Vector3 toadd = new Vector3(desiredSpeed.x - myRigidBody.velocity.x, jump + JumpFormula(), desiredSpeed.z - myRigidBody.velocity.z );
            myRigidBody.AddForce(toadd, ForceMode.VelocityChange);
        }
        float currentSpeed = Pythagore(myRigidBody.velocity.x, myRigidBody.velocity.z);
        DisplaySpeed.text = currentSpeed.ToString("#.##");

        cam.fieldOfView = defaultFieldOfView + (mySpeed>5?mySpeed-5:0) * 2;
        jump = 0.0f;
        
    }
    void Mouselook()
    {
        float rotLeftRight = Input.GetAxis("Mouse X") * MouseSpeed;
        float rotUpDown = Input.GetAxis("Mouse Y") * MouseSpeed;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y+rotLeftRight);
        cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x-rotUpDown,cam.transform.eulerAngles.y);
        
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
            print((1 / (1 + SpaceCounter)));
            print(SpaceCounter);
            return (1 / 2 * (1 + SpaceCounter));
        }
    }

    void CheckSprint()
    {
        if(Input.GetKeyUp(KeyCode.LeftShift))
            speedToDecrease = mySpeed; 
        if (Input.GetKey(KeyCode.LeftShift))
            Sprinting = true;

        else
        {
            Sprinting = false;
        }
        
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&grounded){
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
            countingSpace = false;
        }
    }
    
    void OnCollisionEnter(Collision coll)
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






}