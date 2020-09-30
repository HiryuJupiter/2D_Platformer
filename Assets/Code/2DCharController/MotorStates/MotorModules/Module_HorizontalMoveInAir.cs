using UnityEngine;
using System.Collections;

public class Module_MoveInAir : ModuleBase
{
    public Module_MoveInAir(Player2DController_Motor motor) : base(motor) { }

    float moveXSmoothDampVelocity;

    const float MaxCoyoteDuration = 0.2f;

    public override void ModuleEntry()
    {
        CheckIfJustWalkeOffPlatform();
    }

    public override void TickFixedUpdate()
    {
        //Move
        status.currentVelocity.x = Mathf.SmoothDamp(status.currentVelocity.x, GameInput.MoveX * settings.PlayerMoveSpeed, ref moveXSmoothDampVelocity, settings.SteerSpeedAir * Time.deltaTime);
    }

    void CheckIfJustWalkeOffPlatform()
    {
        if (!status.isOnGround && status.isOnGroundPrevious && !status.isJumping)
        {
            Debug.Log("Just walked off platform");
            status.coyoteTimer = MaxCoyoteDuration;
        }
    }
}