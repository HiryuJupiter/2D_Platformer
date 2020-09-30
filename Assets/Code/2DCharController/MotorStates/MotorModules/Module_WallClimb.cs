using UnityEngine;
using System.Collections;
using UnityEditorInternal;

public class Module_WallClimb : ModuleBase
{
    public Module_WallClimb(Player2DController_Motor motor) : base(motor) { }

    public override void ModuleEntry()
    {
        base.ModuleEntry();
        status.wallStickTimer = settings.WallStickMaxDuration;
    }

    public override void TickUpdate()
    {
        base.TickUpdate();
        if (GameInput.JumpBtnDown)
        {
            WallJump(settings.WallJumpForce);
        }
    }

    public override void TickFixedUpdate()
    {
        status.wallSign = raycaster.GetWallDirSign();
        if (status.wallSign != 0)
        {
            WallSlide();
        }
    }

    public override void ModuleExit()
    {
        base.ModuleExit();
        status.wallStickTimer = -1;
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
            if (status.moveInputSign != 0 && status.moveInputSign != status.wallSign)
            {
                status.wallStickTimer -= Time.deltaTime;
                if (status.wallStickTimer < 0)
                {
                    WallJump(settings.WallDetachForce);
                }
            }
            else
            {
                status.wallStickTimer = settings.WallStickMaxDuration;
            }
        }
    }

    void WallJump(Vector2 v)
    {
        v.x *= -status.wallSign;
        status.isJumping = true;

        status.wallStickTimer = -1f;
        status.currentVelocity = v;

        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;
        motor.SwitchToNewState(MotorStates.Aerial);
    }
}

/*
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
                if  (status.wallStickTimer < 0)
                {
                    DetachFromWall();
                }
            }
            else
            {
                status.wallStickTimer = settings.WallStickMaxDuration;
            }
        }
    }

    void WallJump()
    {
        Vector2 v;
        if (status.wallSign == status.moveSign)
        {
            v = settings.WallJumpUpForce;
        }
        else
        {
            v = settings.WallJumpAwayForce;
        }
        SetJumpVelocity(v);
    }

    void DetachFromWall ()
    {
        SetJumpVelocity(settings.WallDetachForce);
    }

    void SetJumpVelocity (Vector3 v)
    {
        v.x *= -status.wallSign;
        status.isJumping = true;

        status.wallStickTimer = -1f;
        status.currentVelocity = v;

        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;
        motor.SwitchToNewState(MotorStates.Aerial);
    }
 */