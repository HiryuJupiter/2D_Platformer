using UnityEngine;
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

        if (settings.StickyGround)
        {
            modules.Add(new Module_SlopeHandling(motor));
        }
    }

    protected override void Transitions()
    {
        status.wallSign = raycaster.GetWallDirSign();

        if (status.isOnGround && !status.isJumping)
        {
            motor.SwitchToNewState(MotorStates.OnGround);
        }
        else if (!status.isOnGround && !status.isMovingUp 
            && status.wallSign != 0 && status.wallSign + status.moveSign != 0)
        {
            motor.SwitchToNewState(MotorStates.WallClimb);
        }
    }
}