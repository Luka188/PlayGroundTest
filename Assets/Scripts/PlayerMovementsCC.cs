using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementsCC : MonoBehaviour
{
    

    //Floats
    public float speed;
    public float jumpHeight;
    public float airAcceleration;
    public float groundAcceleration;

    float myTimeDelta;
    float currentH;
    float currentV;
    float currentJ = 1;

    //bools
    bool willJump;

    //Unity Stuffs
    Transform myTransform;
    CharacterController CC;
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
        Acceleration = Vector3.ClampMagnitude(Acceleration, speed)*myTimeDelta;
        Acceleration += (myTransform.up * Physics.gravity.y + myTransform.up * currentJ) * myTimeDelta;
        CC.Move(Acceleration);
        
        UpdateJump();
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
            willJump = true;
        }
        if (willJump&&CC.isGrounded)
        {
            currentJ = jumpHeight;
            willJump = false;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            willJump = false;
        }

    }
    void UpdateJump()
    {
        if (currentJ > 1)
            currentJ -= myTimeDelta*jumpHeight;
        else
            currentJ = 1;
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




}
