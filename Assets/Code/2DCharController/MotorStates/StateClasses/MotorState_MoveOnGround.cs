using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MotorState_MoveOnGround", menuName = "Motor States/Move On Ground")]
public class MotorState_MoveOnGround : MotorStateBase
{
    public MotorState_MoveOnGround(Player2DController_Motor motor) : base(motor)
    {
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor),
            new Module_StandardJump(motor),
            new Module_CeilingHitCheck(motor),
            new Module_MoveOnGround(motor),
        };

        if (settings.StickyGround)
        {
            modules.Add(new Module_SlopeHandling(motor));
        }
    }

    protected override void Transitions()
    {
        if (!status.isOnGround)
        {
            motor.SwitchToNewState(MotorStates.Aerial);
        }
    }
}