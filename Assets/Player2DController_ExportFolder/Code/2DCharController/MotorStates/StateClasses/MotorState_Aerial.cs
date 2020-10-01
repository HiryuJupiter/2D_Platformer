using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "MotorState_Aerial", menuName = "Motor States/Aerial")]
public class MotorState_Aerial : MotorStateBase
{
    public MotorState_Aerial(Player2DController_Motor motor) : base(motor)
    {
        modules = new List<ModuleBase>()
        {
            new Module_Gravity(motor),
            new Module_CeilingHitCheck(motor),
            new Module_MoveInAir(motor), 
            new Module_StandardJump(motor),
        };
    }

    protected override void Transitions()
    {
        status.wallSign = raycaster.GetWallDirSign();

        if (status.isOnGround && (!status.isMovingUp || !status.isJumping))
        {
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        //If you hit a wall in mid air and you're moving towards the wall, then go to wallclimb.
        else if (!status.isOnGround && !status.isMovingUp 
            && status.wallSign != 0 && 
            (status.wallSign == status.moveInputSign || status.wallSign == status.velocityXSign))
        {
            motor.SwitchToNewState(MotorStates.WallClimb);
        }
    }
}