using UnityEngine;
using System.Collections;

public class Module_WallClimb : ModuleBase
{
    public Module_WallClimb(Player2DController_Motor motor) : base(motor) { }

    public override void TickFixedUpdate()
    {
        status.wallSign = raycaster.GetWallDirSign();
        if (status.isOnGround)
        {
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        else if (status.isMovingUp || status.wallSign == 0)
        {
            motor.SwitchToNewState(MotorStates.Aerial);
        }
        else if (status.wallSign != 0)
        {
            WallSlide();
            if (GameInput.JumpBtnDown)
            {
                WallJump(status.wallSign, status.moveSign);
            }
        }
    }

    void WallSlide()
    {
        //Limit sliding speed
        if (status.currentVelocity.y < -settings.WallSlideSpeed)
            status.currentVelocity.y = -settings.WallSlideSpeed;

        //Unstuck delay
        if (status.wallStickTimer > 0)
        {
            //Fixed x movement while you have wallStick timer.
            status.currentVelocity.x = 0;

            //If not pressing away from the wall, tick the timer toward unsticking.
            if (status.moveSign != 0 && status.moveSign != status.wallSign)
            {
                status.wallStickTimer -= Time.deltaTime;
            }
            else
            {
                status.wallStickTimer = settings.WallStickMaxDuration;
            }
        }
    }

    void WallJump(int wallSign, int moveSign)
    {
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
        status.currentVelocity = v;
        motor.SwitchToNewState(MotorStates.Aerial);
    }
}