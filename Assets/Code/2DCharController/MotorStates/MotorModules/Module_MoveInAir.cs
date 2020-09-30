using UnityEngine;
using System.Collections;

public class Module_MoveInAir : ModuleBase
{
    public Module_MoveInAir(Player2DController_Motor motor) : base(motor) { }

    float moveXSmoothDampVelocity;

    const float MaxCoyoteDuration = 0.2f;

    public override void ModuleEntry()
    {
        RenewCoyoteTimer();
    }

    public override void TickFixedUpdate()
    {
        status.currentVelocity.x = Mathf.SmoothDamp(status.currentVelocity.x, GameInput.MoveX * settings.PlayerMoveSpeed, ref moveXSmoothDampVelocity, settings.SteerSpeedGround * Time.deltaTime);
    }

    void RenewCoyoteTimer()
    {
        if (!status.isOnGround && status.isOnGroundPrevious && !status.isJumping)
        {
            status.coyoteTimer = MaxCoyoteDuration;
        }
    }
}