using UnityEngine;
using System.Collections;

public class Module_MoveOnGround : ModuleBase
{
    public Module_MoveOnGround(Player2DController_Motor motor) : base(motor) { }

    float moveXSmoothDampVelocity;
    public override void TickFixedUpdate()
    {
        //Move character
        status.currentVelocity.x = Mathf.SmoothDamp(status.currentVelocity.x, GameInput.MoveX * settings.PlayerMoveSpeed, ref moveXSmoothDampVelocity, settings.SteerSpeedGround * Time.deltaTime);
    }
}