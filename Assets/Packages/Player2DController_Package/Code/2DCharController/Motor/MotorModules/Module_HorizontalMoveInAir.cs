using UnityEngine;
using System.Collections;

public class Module_MoveInAir : ModuleBase
{
    public Module_MoveInAir(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    { }

    float moveXSmoothDampVelocity;

    public override void TickUpdate()
    {
        base.TickUpdate();
        feedback.SetFacingBasedOnInput();
    }

    public override void TickFixedUpdate()
    {
        //Move
        motorStatus.currentVelocity.x = Mathf.SmoothDamp(motorStatus.currentVelocity.x, GameInput.MoveX * settings.PlayerMoveSpeed, ref moveXSmoothDampVelocity, settings.SteerSpeedAir * Time.deltaTime);

    }
}