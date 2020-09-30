using UnityEngine;
using System.Collections;

public class Module_Gravity : ModuleBase
{
    public Module_Gravity(Player2DController_Motor motor) : base(motor) { }

    public override void TickFixedUpdate()
    {
        if (status.isOnGround)
        {
            if (status.isFalling && !status.isMoving)
            {
                status.currentVelocity.y = 0;
            }
        }
        else if (!status.isOnGround)
        {
            status.currentVelocity.y -= settings.Gravity * Time.deltaTime;
            status.currentVelocity.y = Mathf.Clamp(status.currentVelocity.y, settings.MaxFallSpeed, status.currentVelocity.y);

            GravityOvershootPrevention();
        }
    }

    //Stops the player from sliding on slopes on the frame that they lands.
    void GravityOvershootPrevention()
    {
        if (status.isFalling && !status.isMoving)
        {
            //If the falling velocity is going below the ground, then reduce the velocity.
            float distance = raycaster.DistanceToGround(-status.currentVelocity.y * Time.deltaTime);
            if (distance > 0)
            {
                //Debug.DrawRay(raycaster.BR, Vector3.right, Color.yellow);
                //Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.green);

                RaycastHit2D right = Physics2D.Raycast(raycaster.BR, Vector2.down, -status.currentVelocity.y * Time.deltaTime, settings.GroundLayer);
                //Debug.DrawRay(right.point, Vector3.right, Color.magenta);

                status.currentVelocity.y = -distance; //THis is absolutely perfct in Non-interpolate. Interpolation does make the character slide upon landing, but this is by far the best option.
                                                      //Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.cyan);
            }
        }
    }
}