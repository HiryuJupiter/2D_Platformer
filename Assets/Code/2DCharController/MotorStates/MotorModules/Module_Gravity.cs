using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
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
            float slopeAngle;


            if (status.isMoving)
            {
                Vector2 vel = status.currentVelocity * Time.deltaTime;
                float velDist = vel.magnitude;
                float distToFloor = raycaster.DistanceAndAngleToGround_Moving(vel, velDist, out slopeAngle);
                if (distToFloor > 0)
                {
                    //Reduce velocity force
                    status.currentVelocity = status.currentVelocity.normalized * (distToFloor - 0.1f) / Time.deltaTime ;
                }
            }
            else
            {
                float distance = raycaster.DistanceAndAngleToGround_NonMoving(-status.currentVelocity.y * Time.deltaTime, out slopeAngle);
                if (distance > 0)
                {
                    //We want the character to have just enough fall speed to land perfectly on ground, however the rigidbody interpolation will cause the character to move a little extra on slop and cause it to slip, so we use a hack, angle * 0.08f, to reduce the fall speed so the slip effect is less apparent.
                    status.currentVelocity.y = -distance / Time.deltaTime + slopeAngle * 0.08f;
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