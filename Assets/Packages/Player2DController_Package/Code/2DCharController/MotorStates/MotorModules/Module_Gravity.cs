﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class Module_Gravity : ModuleBase
{
    public Module_Gravity(Player2DController_Motor motor) : base(motor) { }

    public override void TickFixedUpdate()
    {
        if (status.isOnGround)
        {
            if (status.isFalling && status.moveInputSign == 0)
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
        //If the falling velocity is going below the ground, then reduce the velocity.
        if (status.isFalling)
        {
            if (status.isMoving)
            {
                //If we are moving and falls on a diagonal slope (as opposed to flat ground and vertical walls), the modify velocity
                Vector2 vel = status.currentVelocity * Time.deltaTime;
                float velDist = vel.magnitude;
                float distToFloor = raycaster.MovingDistanceAndAngleToObstacle(vel, velDist, out Vector2 normal);
                float slopeAngle = Vector2.Angle(Vector2.up, normal);
                if (distToFloor > 0 && slopeAngle > 5 && slopeAngle < 80)
                {
                    
                    //Draw ray - velocity
                    //Debug.DrawRay(motor.rb.position, status.currentVelocity, Color.red, 1f);

                    //Reduce velocity force
                    status.currentVelocity = status.currentVelocity.normalized * (distToFloor) / Time.deltaTime ;
                    //status.currentVelocity = status.currentVelocity.normalized * (distToFloor - 0.1f) / Time.deltaTime;

                    //Go to onground state!!

                    //Make the character walk abit along the slope
                    float climbDist = velDist - distToFloor;
                    float targetX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDist * -Mathf.Sign(normal.x);
                    float targetY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDist;
                    //Debug.DrawLine(motor.rb.position, motor.rb.position + new Vector2(targetX, targetY), Color.blue, 1f);
                    status.currentVelocity = (status.currentVelocity.normalized * (distToFloor) + new Vector3(targetX, targetY, 0f)) / Time.deltaTime;

                    //Debug.DrawRay(motor.rb.position, status.currentVelocity, Color.red, 1f);


                        status.isOnGround = true;

                    //Debug.Log("preemptive to ground");
                }
            }
            else
            {
                float distance = raycaster.DistanceAndAngleToGround_NonMoving(-status.currentVelocity.y * Time.deltaTime, out float slopeAngle);
                if (distance > 0)
                {
                    //We want the character to have just enough fall speed to land perfectly on ground, however the rigidbody interpolation will cause the character to move a little extra on slop and cause it to slip, so we use a hack, angle * 0.08f, to reduce the fall speed so the slip effect is less apparent.
                    status.currentVelocity.y = -distance / Time.deltaTime + slopeAngle * 0.08f;
                    status.isOnGround = true;
                }
            }
        }
    }

    IEnumerator DebugVelocity ()
    {
        Debug.DrawRay(raycaster.BR, status.currentVelocity * Time.deltaTime, Color.cyan, 4f);
        yield return null;
        Debug.DrawRay(raycaster.BR, status.currentVelocity * Time.deltaTime, Color.blue, 4f);

    }
}

/*
                     //Climb slope
                    if (Mathf.Sign(status.currentVelocity.x) != Mathf.Sign(slopeAngle))
                    {
                        float climbDist = velDist - distToFloor;
                        float targetX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDist * status.moveInputSign;
                        float targetY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDist;
                        Debug.DrawLine(motor.rb.position, new Vector2(targetX, targetY), Color.blue, 4f);
                        status.currentVelocity = (new Vector2(targetX, targetY) - motor.rb.position) ;
                    }
 */

/*
         if (status.isFalling && !status.isMoving)
        {
            //If the falling velocity is going below the ground, then reduce the velocity.
            float angle;
            float distance = raycaster.DistanceToGroundAndAngle(-status.currentVelocity.y * Time.deltaTime, out angle);
            if (distance > 0)
            {
                //We want the character to have just enough fall speed to land perfectly on ground, however the rigidbody interpolation will cause the character to move a little extra on slop and cause it to slip, so we use a hack, angle * 0.08f, to reduce the fall speed so the slip effect is less apparent.
                status.currentVelocity.y = -distance / Time.deltaTime + angle * 0.08f;
            }
        }
 */

/*
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

                status.currentVelocity.y = -distance / Time.deltaTime;
                //Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.cyan);

                //-distance / Time.deltaTime;  is good for flat surface but slides on slope
                //-distance; is good for slopes 
                //We use the flat surface version because 
            }
        }
    }
 */