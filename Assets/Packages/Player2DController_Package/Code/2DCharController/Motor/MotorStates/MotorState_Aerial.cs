using UnityEngine;
using System.Collections.Generic;


public class MotorState_Aerial : MotorStateBase
{
    public MotorState_Aerial(Player2DMotor motor, Player2DFeedbacks feedbacks) : base(motor, feedbacks)
    {
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor, feedbacks),
            new Module_CeilingHitCheck(motor, feedbacks),
            new Module_MoveInAir(motor, feedbacks),
            new Module_StandardJump(motor, feedbacks),
        };
    }

    public override void StateEntry()
    {
        base.StateEntry();
        feedback.Animator.SetOnGround(false);
    }

    public override void TickUpdate()
    {
        base.TickUpdate();
        feedback.Animator.SetYVelocity(motorStatus.currentVelocity.y);
    }

    protected override void Transitions()
    {
        motorStatus.wallSign = raycaster.GetWallDirSign();

        if (motorStatus.isOnGround && (!motorStatus.isMovingUp || !motorStatus.isJumping))
        {
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        //If you hit a wall in mid air and you're moving towards the wall, then go to wallclimb.
        else if (!motorStatus.isOnGround && !motorStatus.isMovingUp 
            && motorStatus.wallSign != 0 && 
            (motorStatus.wallSign == motorStatus.moveInputSign || motorStatus.wallSign == motorStatus.velocityXSign))
        {
            motor.SwitchToNewState(MotorStates.WallClimb);
        }
    }
}