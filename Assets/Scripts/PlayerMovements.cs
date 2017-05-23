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
    float lastVertical = 0;
    public float speed;
    public float MouseSpeed;
    public float JumpForce;
    public float mySpeed = 5;
    public float AirControl = 2;
   
    //bools
    bool grounded = false;
    bool countingSpace;
    bool JumpWhenPossible = false;
    public bool CanControl = true;
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
        
        
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        CheckJump();

        mySpeed = 10;

        if (CanControl)
        {
            Vector3 desiredSpeed = transform.forward * GetNewVertical(vertical) * mySpeed;
            Vector3 toAdd = new Vector3(desiredSpeed.x - myRigidBody.velocity.x, jump + JumpFormula(), desiredSpeed.z - myRigidBody.velocity.z);
            myRigidBody.AddForce(toAdd, ForceMode.VelocityChange);
        }
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
    float GetNewVertical(float vertical)
    {
        
        vertical =  Mathf.MoveTowards(lastVertical, vertical, Time.deltaTime *(vertical==0?AirControl/2:AirControl));
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