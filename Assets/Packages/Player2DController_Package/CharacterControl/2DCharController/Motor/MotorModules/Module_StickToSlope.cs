using UnityEngine;
using System.Collections;

public class Module_StickToSlope : ModuleBase
{
    const float SkinWidth = 0.005f;

    float decendSlopeMaxCheckDist;


    public Module_StickToSlope(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    {
        decendSlopeMaxCheckDist = settings.PlayerMoveSpeed * Mathf.Tan(settings.MaxSlopeAngle * Mathf.Deg2Rad);
    }

    public override void TickFixedUpdate()
    {
        if (motorStatus.isJumping)
        {
            return;
        }

        motorStatus.climbingSlope = false;
        motorStatus.descendingSlope = false;

        if (motorStatus.isMoving)
        {
            //Ascending
            Vector2 frontfoot = motorStatus.moveInputSign > 0 ? raycaster.BR : raycaster.BL;
            StickToAscendingSlope(frontfoot);

            //Only allow decending slope when not currently ascending.
            if (!motorStatus.climbingSlope)
            {
                //Descending
                Vector2 backfoot = motorStatus.moveInputSign > 0 ? raycaster.BL : raycaster.BR;
                StickToDecendingSlope(backfoot);

                //This prevents "car-flys-over-ramp" effect after finish climbing slope.
                if (!motorStatus.descendingSlope && !motorStatus.isOnGround)
                {
                    //Stick to ground
                    if (motorStatus.currentVelocity.y > 0f)
                    {
                        motorStatus.currentVelocity.y = 0f;
                    }
                }
            }
        }
        else
        {
            if (motorStatus.isOnGround)
            {
                //Don't let player slide down a slope by gravity.
                motorStatus.currentVelocity.y = 0f;
            }
        }
    }

    void StickToAscendingSlope(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.right * motorStatus.moveInputSign, Mathf.Abs(motorStatus.currentVelocity.x) * Time.deltaTime, settings.GroundLayer);

        //Debug.DrawRay(origin, new Vector3(currentVelocity.x * Time.deltaTime, 0f, 0f), Color.cyan);

        if (hit)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            if (slopeAngle != 0 && slopeAngle < settings.MaxSlopeAngle)
            {
                motorStatus.climbingSlope = true;
                Vector3 newVelocity = Vector3.zero;
                float gapDist = 0f;
                //If there is space between you and the slope, then move right up against it.
                if (slopeAngle != motorStatus.slopeAngleOld) //For optimization, only do once per slope
                {
                    gapDist = hit.distance - SkinWidth;
                    newVelocity.x = gapDist / Time.deltaTime * motorStatus.moveInputSign;
                }
                //Take the full VelocityX, minus the gap distance, then use the remaining velocity X...
                //...to calculate slope climbing. 
                float climbDistance = settings.PlayerMoveSpeed - gapDist; //climbDistance is also the hypotenues
                float displaceX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDistance * motorStatus.moveInputSign;
                float displaceY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDistance;
                newVelocity.x += displaceX;
                newVelocity.y = displaceY;

                motorStatus.currentVelocity = newVelocity;
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
            motorStatus.slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            //If the slope is less than maxSlope angle
            if (motorStatus.slopeAngle != 0 && motorStatus.slopeAngle < settings.MaxSlopeAngle)
            {
                //See if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
                if (Mathf.Sign(hit.normal.x) == motorStatus.moveInputSign)
                {
                    motorStatus.descendingSlope = true;
                    //Check if we are standing close enough to the platform to begin decend calculation. 
                    //float descendableRange = decendSlopeCheckDist ;
                    //if (hit.distance - SkinWidth < descendableRange)
                    {
                        //Specify the decend amount
                        //Btw we're using max move speed (moveSpeed) instead of currentVelocity.x because it is reduced by smoothdamp.
                        motorStatus.currentVelocity.x = Mathf.Cos(motorStatus.slopeAngle * Mathf.Deg2Rad) * settings.PlayerMoveSpeed * motorStatus.moveInputSign;
                        motorStatus.currentVelocity.y = -Mathf.Sin(motorStatus.slopeAngle * Mathf.Deg2Rad) * settings.PlayerMoveSpeed;
                        //currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
                        if (motorStatus.slopeAngle != motorStatus.slopeAngleOld)
                        {
                            //Make the player move towards the slop if it is hovering above it
                            //We use slopAngleOld for performance optimization
                            motorStatus.currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
                        }
                    }
                }
            }
        }
    }
}