using UnityEngine;
using System.Collections.Generic;

/*=== TERMINOLOGIES ===
 * COYOTE TIME: After walking off a platform, the player has a brief moment where they can still jump.
 * JUMP QUEUE TIMER: The player can cache the jump command for a brief moment while midair. If it is cached right before landing, the player will automatically jump after landing. 
 */

[CreateAssetMenu(fileName = "MotorState_Aerial", menuName = "Motor States/Aerial")]
public class MotorState_Aerial : MotorStateBase
{
    const float MaxCoyoteDuration = 0.2f;


    public MotorState_Aerial(Player2DController_Motor motor) : base(motor)
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

    #region Public 
    public override void TickFixedUpdate()
    {
        base.TickFixedUpdate();
        if (status.isOnGround && !status.isJumping)
        {
            motor.SwitchToNewState(MotorStates.OnGround);
        }
    }

    public override void StateExit()
    {
        base.StateExit();
        status.isJumping = false;
        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;
    }
    #endregion
}