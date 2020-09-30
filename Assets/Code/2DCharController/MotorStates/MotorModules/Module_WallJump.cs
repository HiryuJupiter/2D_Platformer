using UnityEngine;
using System.Collections;

public class Module_WallJump : ModuleBase
{
    public Module_WallJump(Player2DController_Motor motor) : base(motor)
    {
    }

    public override void TickUpdate()
    {
        Debug.Log("Module_WallJump TickUpdate");
        if (GameInput.JumpBtnDown)
        {
            WallJump(status.wallSign, status.moveSign);
        }
    }

    void WallJump(int wallSign, int moveSign)
    {
        Debug.Log("WALL JUMP");
        Vector2 v;
        if (wallSign == moveSign)
        {
            v = settings.WallJumpClimbUp;
        }
        else if (moveSign == 0)
        {
            v = settings.WallJumpNormal;
        }
        else
        {
            v = settings.WallJumpAway;
        }
        v.x *= -wallSign;
        status.isJumping = true;

        status.wallStickTimer = -1f;
        status.currentVelocity = v;

        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;
        motor.SwitchToNewState(MotorStates.Aerial);
    }
}