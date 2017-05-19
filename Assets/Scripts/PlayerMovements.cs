using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerMovements : MonoBehaviour
{
    //ints

    //floats
    float maxSlope = 50;
    float maxYVel = 0;
    float currentT = 0;
    public float jump;
    public float speed;
    public float MouseSpeed;
    public float JumpForce;
    //bools
    bool grounded = false;

    //Unity Stuffs
    Rigidbody myRigidBody;
    public Camera cam;
    public AnimationCurve SlopeCurveModifier ;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        
    }
    void Update()
    {

        Mouselook();
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        CheckJump();
        if (grounded)
        {
            
            
            myRigidBody.AddForce(new Vector3(vertical*SlopeCurveModifier(, ForceMode.VelocityChange);

        }
        else 
        {
            Vector3 desiredmove = (transform.forward * vertical + transform.right * horizontal) * speed;
            desiredmove = Vector3.ClampMagnitude(desiredmove, speed);
            Vector3 forcetoadd = new Vector3(desiredmove.x - myRigidBody.velocity.x, jump*5, desiredmove.z - myRigidBody.velocity.z) / 5;
            myRigidBody.AddForce(forcetoadd, ForceMode.VelocityChange);
        }
        currentT += Time.deltaTime;
        jump = 0.0f;

    }
    void Mouselook()
    {
        
        float rotLeftRight = Input.GetAxis("Mouse X") * MouseSpeed;
        float rotUpDown = Input.GetAxis("Mouse Y") * MouseSpeed;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y+rotLeftRight);
        cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x-rotUpDown,cam.transform.eulerAngles.y);
        
    }
    void checkCurve()
    {
        if()
    }
    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&grounded){
            jump = JumpForce;
            grounded = false;
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






}