using UnityEngine;
using System.Collections.Generic;

public class MotorState_WallClimb : MotorStateBase
{
    Module_WallClimb wallClimb;
    public MotorState_WallClimb(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    {
        wallClimb = new Module_WallClimb(motor, this.feedback);
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor, feedback),
            new Module_CeilingHitCheck(motor, feedback),
            wallClimb
        };
    }

    public override void StateEntry()
    {
        base.StateEntry();
        motorStatus.isWallSliding = true;
        motorStatus.isJumping = false;
        motorStatus.jumpQueueTimer = -1f;

        //Immediately climb wall, instead of waiting for next frame.
        wallClimb.TickFixedUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
        motorStatus.isWallSliding = false;
    }

    protected override void Transitions()
    {
        if (motorStatus.isOnGround)
        {
            motorStatus.isJumping = false;
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        else if (motorStatus.wallSign == 0 || motorStatus.wallStickTimer <= 0f || motorStatus.isMovingUp)
        {
            motor.SwitchToNewState(MotorStates.Aerial);

            //If we slid off the wall, then give coyote time for the player to recuperate.
            if (!motorStatus.isJumping)
            {
                motorStatus.coyoteTimer = settings.MaxCoyoteDuration;
            }
        }
        //else if (status.wallSign == 0 )
        //{
        //    //Debug.Log("1");
        //    motor.SwitchToNewState(MotorStates.Aerial);
        //}
        //else if (status.wallStickTimer <= 0f)
        //{
        //    Debug.Log("2");
        //    motor.SwitchToNewState(MotorStates.Aerial);
        //}
        //else if (status.isMovingUp)
        //{
        //    Debug.Log("3");
        //    motor.SwitchToNewState(MotorStates.Aerial);
        //}
    }
}