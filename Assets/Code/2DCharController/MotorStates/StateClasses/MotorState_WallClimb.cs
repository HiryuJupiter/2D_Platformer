using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MotorState_WallClimb", menuName = "Motor States/Wall Climb")]
public class MotorState_WallClimb : MotorStateBase
{
    public MotorState_WallClimb(Player2DController_Motor motor) : base(motor)
    {
        modules = new List<ModuleBase>()
        {
            new Module_WallClimb(motor),
            new Module_WallJump(motor),
            new Module_Gravity(motor),
            new Module_CeilingHitCheck(motor)
        };
    }

    public override void StateEntry()
    {
        base.StateEntry();
        status.isWallSliding = true;
    }

    public override void StateExit()
    {
        base.StateExit();
        status.isWallSliding = false;
    }

    protected override void Transitions()
    {
        if (status.isOnGround)
        {
            status.isJumping = false;
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        else if (status.wallSign == 0 || status.wallStickTimer <= 0f)
        {
            motor.SwitchToNewState(MotorStates.Aerial);
        }
    }


}