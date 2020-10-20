using UnityEngine;
using System.Collections;

public class Module_WallClimb : ModuleBase
{
    public Module_WallClimb(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    { }

    public override void ModuleEntry()
    {
        base.ModuleEntry();
        motorStatus.wallStickTimer = settings.WallStickMaxDuration;
    }

    public override void TickUpdate()
    {
        base.TickUpdate();
        if (GameInput.JumpBtnDown)
        {
            if (motorStatus.wallSign == motorStatus.moveInputSign)
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
        motorStatus.wallSign = motor.raycaster.GetWallDirSign();
        if (motorStatus.wallSign != 0)
        {
            WallSlide();
        }
    }

    public override void ModuleExit()
    {
        base.ModuleExit();
        motorStatus.wallStickTimer = -1;
    }

    void WallSlide()
    {
        //Limit downward sliding speed on wall
        SetWallSlideSpeed(GameInput.PressedDown ? settings.WallSlideSpeedFast : settings.WallSlideSpeedSlow);

        //Unstuck delay (to allow the player press away from wall and jump)
        if (motorStatus.wallStickTimer > 0)
        {
            //Freeze x movement while you are stuck to wall.
            motorStatus.currentVelocity.x = 0;

            //If pressing away from the wall, tick the timer toward unsticking.
            if (motorStatus.moveInputSign != 0 && motorStatus.moveInputSign != motorStatus.wallSign)
            {
                motorStatus.wallStickTimer -= Time.deltaTime;
                if (motorStatus.wallStickTimer < 0)
                {
                    //Naturally detach the player.
                    WallJump(settings.WallDetachForce, false);
                }
            }
            else
            {
                motorStatus.wallStickTimer = settings.WallStickMaxDuration;
            }
        }
    }

    void WallJump(Vector2 v, bool isRealJumping = true)
    {
        //If we slid off the wall, the player gets to jump immediately to recUperate.

        v.x *= -motorStatus.wallSign;
        motorStatus.isJumping = isRealJumping;

        motorStatus.wallStickTimer = -1f;
        motorStatus.currentVelocity = v;

        motorStatus.jumpQueueTimer = -1f;
        motorStatus.coyoteTimer = isRealJumping ? - 1f : settings.MaxCoyoteDuration;
        motor.SwitchToNewState(MotorStates.Aerial);
    }

    void SetWallSlideSpeed (float maxSpeed)
    {
        if (motorStatus.currentVelocity.y < -maxSpeed)
            motorStatus.currentVelocity.y = -maxSpeed;
    }
}
