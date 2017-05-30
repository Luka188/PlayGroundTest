using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementState {

    static bool sliding;
    public static bool Sliding { get { return sliding; } set { if (value) ResetMovement(); sliding = value; } }
    static bool wallJumping;
    public static bool WallJumping { get { return wallJumping; } set {if(value) ResetMovement(); wallJumping = value; } }
    static bool velJumping;
    public static bool VelJumping { get { return velJumping; } set { velJumping = value; } }
    static bool groundSiding;
    public static bool GroundSliding { get { return groundSiding; } set { if (value) ResetMovement(); groundSiding = value; } }


    static void ResetMovement()
    {
        sliding = false;
        wallJumping = false;
        groundSiding = false;
    }
}
