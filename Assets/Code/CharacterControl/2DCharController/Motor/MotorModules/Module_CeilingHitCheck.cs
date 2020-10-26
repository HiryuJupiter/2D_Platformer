using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Module_CeilingHitCheck : ModuleBase
{
    public Module_CeilingHitCheck(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    { }

    public override void TickFixedUpdate()
    {
        if (motorStatus.isMovingUp)
        {
            //Nudges the character to the side if about to hit the edge of a platform above.
            float nudgeX = raycaster.CheckForCeilingSideNudge(motorStatus.currentVelocity.y * Time.deltaTime);
            if (nudgeX != 0f)
            {
                motor.ForceNudge(new Vector2(nudgeX, 0f));
            }

            //If hits ceiling at close range
            if (raycaster.HitsCeiling())
            {
                motorStatus.currentVelocity.y = 0f;
                motorStatus.isJumping = false;
            }
        }
    }
}