using UnityEngine;
using System.Collections;

public class Module_SlopeHandling : ModuleBase
{
    const float SkinWidth = 0.005f;

    float decendSlopeMaxCheckDist;


    public Module_SlopeHandling(Player2DController_Motor motor) : base(motor)
    {
        decendSlopeMaxCheckDist = settings.PlayerMoveSpeed * Mathf.Tan(settings.MaxSlopeAngle * Mathf.Deg2Rad);
    }

    public override void TickFixedUpdate()
    {
        if (status.isJumping)
        {
            return;
        }

        status.climbingSlope = false;
        status.descendingSlope = false;

        if (status.isMoving)
        {
            //Ascending
            Vector2 frontfoot = status.moveSign > 0 ? raycaster.BR : raycaster.BL;
            StickToAscendingSlope(frontfoot);

            //Only allow decending slope when not currently ascending.
            if (!status.climbingSlope)
            {
                //Descending
                Vector2 backfoot = status.moveSign > 0 ? raycaster.BL : raycaster.BR;
                StickToDecendingSlope(backfoot);

                //This prevents "car-flys-over-ramp" effect after finish climbing slope.
                if (!status.descendingSlope && !status.isOnGround)
                {
                    //Stick to ground
                    if (status.currentVelocity.y > 0f)
                    {
                        status.currentVelocity.y = 0f;
                    }
                }
            }
        }
        else
        {
            if (status.isOnGround)
            {
                //Don't let player slide down a slope by gravity.
                status.currentVelocity.y = 0f;
            }
        }
    }

    void StickToAscendingSlope(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.right * status.moveSign, Mathf.Abs(status.currentVelocity.x) * Time.deltaTime, settings.GroundLayer);

        //Debug.DrawRay(origin, new Vector3(currentVelocity.x * Time.deltaTime, 0f, 0f), Color.cyan);

        if (hit)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            if (slopeAngle != 0 && slopeAngle < settings.MaxSlopeAngle)
            {
                status.climbingSlope = true;
                Vector3 newVelocity = Vector3.zero;
                float gapDist = 0f;
                //If there is space between you and the slope, then move right up against it.
                if (slopeAngle != status.slopeAngleOld) //For optimization, only do once per slope
                {
                    gapDist = hit.distance - SkinWidth;
                    newVelocity.x = gapDist / Time.deltaTime * status.moveSign;
                }
                //Take the full VelocityX, minus the gap distance, then use the remaining velocity X...
                //...to calculate slope climbing. 
                float climbDistance = settings.PlayerMoveSpeed - gapDist; //climbDistance is also the hypotenues
                float displaceX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDistance * status.moveSign;
                float displaceY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDistance;
                newVelocity.x += displaceX;
                newVelocity.y = displaceY;

                status.currentVelocity = newVelocity;
            }
        }
    }

    void StickToDecendingSlope(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, decendSlopeMaxCheckDist * Time.deltaTime, settings.GroundLayer);
        //Debug.DrawRay(origin, Vector3.down * decendSlopeCheckDist * Time.deltaTime, Color.red, 0.5f);

        if (hit)
        {
            //Debug.DrawRay(hit.point, hit.normal, Color.magenta, 0.5f);
            status.slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            //If the slope is less than maxSlope angle
            if (status.slopeAngle != 0 && status.slopeAngle < settings.MaxSlopeAngle)
            {
                //See if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
                if (Mathf.Sign(hit.normal.x) == status.moveSign)
                {
                    status.descendingSlope = true;
                    //Check if we are standing close enough to the platform to begin decend calculation. 
                    //float descendableRange = decendSlopeCheckDist ;
                    //if (hit.distance - SkinWidth < descendableRange)
                    {
                        //Specify the decend amount
                        //Btw we're using max move speed (moveSpeed) instead of currentVelocity.x because it is reduced by smoothdamp.
                        status.currentVelocity.x = Mathf.Cos(status.slopeAngle * Mathf.Deg2Rad) * settings.PlayerMoveSpeed * status.moveSign;
                        status.currentVelocity.y = -Mathf.Sin(status.slopeAngle * Mathf.Deg2Rad) * settings.PlayerMoveSpeed;
                        //currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
                        if (status.slopeAngle != status.slopeAngleOld)
                        {
                            //Make the player move towards the slop if it is hovering above it
                            //We use slopAngleOld for performance optimization
                            status.currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
                        }
                    }
                }
            }
        }
    }
}