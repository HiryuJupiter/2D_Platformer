using UnityEngine;
using System.Collections;

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
            if (status.wallSign == status.moveInputSign)
            {
                WallJump(settings.WallJumpUpForce);
            }
            else
            {
                WallJump(settings.WallJumpAwayForce);
            }
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
        //Limit downward sliding speed on wall
        SetWallSlideSpeed((GameInput.MoveY < -0.1f) ? settings.WallSlideSpeedFast : settings.WallSlideSpeedSlow);

        //Unstuck delay (to allow the player press away from wall and jump)
        if (status.wallStickTimer > 0)
        {
            //Freeze x movement while you are stuck to wall.
            status.currentVelocity.x = 0;

            //If pressing away from the wall, tick the timer toward unsticking.
            if (status.moveInputSign != 0 && status.moveInputSign != status.wallSign)
            {
                status.wallStickTimer -= Time.deltaTime;
                if (status.wallStickTimer < 0)
                {
                    //Naturally detach the player.
                    WallJump(settings.WallDetachForce, false);
                }
            }
            else
            {
                status.wallStickTimer = settings.WallStickMaxDuration;
            }
        }
    }

    void WallJump(Vector2 v, bool isRealJumping = true)
    {
        //If we slid off the wall, the player gets to jump immediately to recUperate.

        v.x *= -status.wallSign;
        status.isJumping = isRealJumping;

        status.wallStickTimer = -1f;
        status.currentVelocity = v;

        status.jumpQueueTimer = -1f;
        status.coyoteTimer = isRealJumping ? - 1f : settings.MaxCoyoteDuration;
        motor.SwitchToNewState(MotorStates.Aerial);
    }

    void SetWallSlideSpeed (float maxSpeed)
    {
        if (status.currentVelocity.y < -maxSpeed)
            status.currentVelocity.y = -maxSpeed;
    }
}
