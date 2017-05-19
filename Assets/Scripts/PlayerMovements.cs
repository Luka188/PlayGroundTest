using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerMovements : MonoBehaviour
{
    //ints

    //floats
    float maxSlope = 50;
    float jump = 0;
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
        
        float myspeed = SlopeCurveModifier.Evaluate(Pythagore(myRigidBody.velocity.x, myRigidBody.velocity.z));

        if (jump > 0)
        {
            float prevmagn = myRigidBody.velocity.magnitude + 0.5f;
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.AddForce(transform.forward * vertical * prevmagn+ Vector3.up * jump, ForceMode.VelocityChange);
            
        }
        else
        {
            myRigidBody.AddForce(transform.forward * vertical * myspeed * Time.deltaTime + transform.right * horizontal *myspeed* Time.deltaTime, ForceMode.VelocityChange);
        }
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
    
    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&grounded){
            jump = JumpForce;
            grounded = false;
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