using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MotorState_WallClimb", menuName = "Motor States/Wall Climb")]
public class MotorState_WallClimb : MotorStateBase
{
    Module_WallClimb wallClimb;
    Module_WallClimb gravity;

    public MotorState_WallClimb(Player2DController_Motor motor) : base(motor)
    {
        modules = new List<ModuleBase>()
        {
            new Module_WallClimb(motor),
            new Module_Gravity(motor),
        };
    }

    public override void StateEntry()
    {
        base.StateEntry();
        status.wallStickTimer = settings.WallStickMaxDuration;
    }

    public override void TickFixedUpdate()
    {
        base.TickFixedUpdate();
        gravity.TickFixedUpdate();
        wallClimb.TickFixedUpdate();
    }

    public override void StateExit()
    {
    }
}