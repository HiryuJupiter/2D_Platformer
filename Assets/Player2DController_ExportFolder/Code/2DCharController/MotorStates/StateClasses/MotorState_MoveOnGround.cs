using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MotorState_MoveOnGround", menuName = "Motor States/Move On Ground")]
public class MotorState_MoveOnGround : MotorStateBase
{
    Module_StickToSlope stickToSlope;
    public MotorState_MoveOnGround(Player2DController_Motor motor) : base(motor)
    {
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor),
            new Module_CeilingHitCheck(motor),
            new Module_MoveOnGround(motor),
            new Module_StandardJump(motor),
        };

        if (settings.StickyGround)
        {
            stickToSlope = new Module_StickToSlope(motor);
            modules.Add(stickToSlope);
        }
    }

    public override void StateEntry()
    {
        base.StateEntry();

        //Immediately stick to slope, don't wait until next frame.
        if (settings.StickyGround)
            stickToSlope.TickFixedUpdate();
    }

    protected override void Transitions()
    {
        if (!status.isOnGround)
        {
            motor.SwitchToNewState(MotorStates.Aerial);
        }
    }
}