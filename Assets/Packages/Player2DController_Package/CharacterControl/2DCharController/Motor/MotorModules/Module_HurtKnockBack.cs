using UnityEngine;
using System.Collections;

public class Module_HurtKnockBack : ModuleBase
{
    public Module_HurtKnockBack(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    { }

    float moveXSmoothDampVelocity;

    public override void ModuleEntry()
    {
        base.ModuleEntry();

        //motorStatus.currentVelocity.x = motorStatus.
        //motorStatus.currentVelocity = settings.HurtForce;
    }

    public override void TickFixedUpdate()
    {
        motorStatus.currentVelocity.x = Mathf.SmoothDamp(motorStatus.currentVelocity.x, 0f, ref moveXSmoothDampVelocity, settings.HurtSlideSpeed * Time.deltaTime);
    }

    public override void ModuleExit()
    {
        base.ModuleExit();
        motorStatus.wallStickTimer = -1;
    }
}
