﻿using UnityEngine;
using System.Collections.Generic;

/*=== TERMINOLOGIES ===
 * COYOTE TIME: After walking off a platform, the player has a brief moment where they can still jump.
 * JUMP QUEUE TIMER: The player can cache the jump command for a brief moment while midair. If it is cached right before landing, the player will automatically jump after landing. 
 */

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

        //if (settings.StickyGround)
        //{
        //    modules.Add(new Module_StickToSlope(motor));
        //}
    }

    protected override void Transitions()
    {
        status.wallSign = raycaster.GetWallDirSign();

        if (status.isOnGround && (!status.isMovingUp || !status.isJumping))
        {
            Debug.Log("aerial to ground");
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        //If hits a wall in air, and either you're pressing towards the wall or
        //your momentum is going towards the wall, go to wallclimb.
        else if (!status.isOnGround && !status.isMovingUp 
            && status.wallSign != 0 && 
            (status.wallSign == status.moveInputSign || status.wallSign == status.velocityXSign))
        {
            motor.SwitchToNewState(MotorStates.WallClimb);
        }
    }
}