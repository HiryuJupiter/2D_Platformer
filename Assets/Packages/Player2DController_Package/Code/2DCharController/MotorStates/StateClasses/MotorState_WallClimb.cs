using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MotorState_WallClimb", menuName = "Motor States/Wall Climb")]
public class MotorState_WallClimb : MotorStateBase
{
    Module_WallClimb wallClimb;
    public MotorState_WallClimb(Player2DController_Motor motor) : base(motor)
    {
        wallClimb = new Module_WallClimb(motor);
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor),
            new Module_CeilingHitCheck(motor),
            wallClimb
        };
    }

    public override void StateEntry()
    {
        base.StateEntry();
        status.isWallSliding = true;
        status.isJumping = false;
        status.jumpQueueTimer = -1f;

        //Immediately climb wall, instead of waiting for next frame.
        wallClimb.TickFixedUpdate();
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
        else if (status.wallSign == 0 || status.wallStickTimer <= 0f || status.isMovingUp)
        {
            motor.SwitchToNewState(MotorStates.Aerial);
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