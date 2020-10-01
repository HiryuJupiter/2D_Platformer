using UnityEngine;

public class MotorStatus
{
    //Move
    public bool isOnGround;
    public bool isOnGroundPrevious;
    public bool isJumping;
    public int moveInputSign;
    public int velocityXSign;
    public Vector3 currentVelocity;

    //Jump
    public float coyoteTimer;
    public float jumpQueueTimer;

    //Slope handing
    public bool descendingSlope;
    public bool climbingSlope;
    public float slopeAngle;
    public float slopeAngleOld;

    //Wall climb 
    public int wallSign;
    public float wallStickTimer;
    public bool isWallSliding;

    //Properties
    public bool isFalling => currentVelocity.y < 0f;
    public bool isMovingUp => currentVelocity.y > 0f;
    public bool isMoving => moveInputSign != 0;
    public bool canJump => isOnGround || isWallSliding || (coyoteTimer > 0f && !isJumping);
    public bool justLanded => !isOnGroundPrevious && isOnGround;
    public void CacheCurrentValuesToOld()
    {
        isOnGroundPrevious = isOnGround;
        slopeAngleOld = slopeAngle;
    }
}