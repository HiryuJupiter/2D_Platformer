using UnityEngine;
using System.Collections.Generic;

public class MotorState_MoveOnGround : MotorStateBase
{
    Module_StickToSlope stickToSlope;
    public MotorState_MoveOnGround(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    {
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor, feedback),
            new Module_CeilingHitCheck(motor, feedback),
            new Module_MoveOnGround(motor, feedback),
            new Module_StandardJump(motor, feedback),
        };

        if (settings.StickyGround)
        {
            stickToSlope = new Module_StickToSlope(motor, this.feedback);
            modules.Add(stickToSlope);
        }
    }

    public override void StateEntry()
    {
        base.StateEntry();

        //Immediately stick to slope, don't wait until next frame.
        if (settings.StickyGround)
            stickToSlope.TickFixedUpdate();
        feedback.Animator.SetOnGround(true);
    }

    public override void TickUpdate()
    {
        base.TickUpdate();
        feedback.Animator.SetXVelocity(Mathf.Abs(motorStatus.currentVelocity.x));
    }

    protected override void Transitions()
    {
        //Go to aerial state if not on ground in current frame and in previous frame, or if moving up (jumping). We check for previous frame because when the player lands on a slope, they can come in and out of isOnGround status for 1 frame.
        if (!motorStatus.isOnGround && (motorStatus.currentVelocity.y > 0f || !motorStatus.isOnGroundPrevious))
        {
            motor.SwitchToNewState(MotorStates.Aerial);
        }
    }
}