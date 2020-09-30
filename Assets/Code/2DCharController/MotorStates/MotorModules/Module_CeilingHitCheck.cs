using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Module_CeilingHitCheck : ModuleBase
{
    Rigidbody2D rb;
    public Module_CeilingHitCheck(Player2DController_Motor motor) : base(motor)
    {
        rb = motor.Rb;
    }
    public override void TickFixedUpdate()
    {
        if (status.isMovingUp)
        {
            //Nudes the character to the side if about to hit the edge of a platform above.
            float nudgeX = raycaster.CheckForCeilingSideNudge(status.currentVelocity.y * Time.deltaTime);
            if (nudgeX != 0f)
            {
                Vector3 p = rb.position;
                p.x += nudgeX;
                rb.position = p;
            }

            //If hits ceiling at close range
            if (raycaster.HitsCeiling())
            {
                status.currentVelocity.y = 0f;
                status.isJumping = false;
            }
        }
    }
}