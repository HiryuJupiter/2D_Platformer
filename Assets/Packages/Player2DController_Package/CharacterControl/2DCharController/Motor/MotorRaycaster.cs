using UnityEngine;
using System.Collections;

/*
 What is side nudge? It's when the player controller hits the side of a platform 
above it by just a tiny bit, and the game automatically nudges the player out of the
way.
 */

[RequireComponent(typeof(Collider2D))]
public class MotorRaycaster : MonoBehaviour
{
    //Cache
    LayerMask groundLayer;

    //Offsets
    Vector2 offset_BL; //Offset bottom left
    Vector2 offset_BR; //Offset bottom right
    Vector2 offset_TL_outer; //Offset top left
    Vector2 offset_TR_outer; //Offset top right
    Vector2 offset_TL_inner;
    Vector2 offset_TR_inner;

    //Const
    const float checkDist = 0.06f;
    const float sideNudgeDist = 0.2f;

    #region Properties
    public bool IsOnGround => OnGroundCheck();

    //The final world positions (not offsets)
    public Vector2 BL { get; private set; }
    public Vector2 BR { get; private set; }
    public Vector2 TL_outer { get; private set; }
    public Vector2 TR_outer { get; private set; }
    public Vector2 TL_inner { get; private set; }
    public Vector2 TR_inner { get; private set; }
    #endregion

    #region MonoBehavior
    void Awake()
    {
        //Initialize offset cache
        Bounds bounds = GetComponent<Collider2D>().bounds;
        float x = bounds.extents.x - 0.005f;
        float y = bounds.extents.y - 0.005f;
        offset_BL = new Vector2(-x, -y);
        offset_BR = new Vector2(x, -y);
        offset_TL_outer = new Vector2(-x, y);
        offset_TR_outer = new Vector2(x, y);
        offset_TL_inner = new Vector2(-x + sideNudgeDist, y);
        offset_TR_inner = new Vector2(x - sideNudgeDist, y);

    }

    void Start()
    {
        groundLayer = GameSettings.instance.GroundLayer;
    }
    #endregion

    #region Public
    public void UpdateOriginPoints()
    {
        BL = (Vector2)transform.position + offset_BL;
        BR = (Vector2)transform.position + offset_BR;
        TL_outer = (Vector2)transform.position + offset_TL_outer;
        TR_outer = (Vector2)transform.position + offset_TR_outer;
        TL_inner = (Vector2)transform.position + offset_TL_inner;
        TR_inner = (Vector2)transform.position + offset_TR_inner;

        //Debug
        //Debug.DrawRay(BL, Vector3.left, Color.blue);
        //Debug.DrawRay(BR, Vector3.right, Color.blue);
        //Debug.DrawRay(TL_outer, Vector3.left, Color.blue);
        //Debug.DrawRay(TR_outer, Vector3.right, Color.blue);
        //Debug.DrawRay(TL_inner, Vector3.left, Color.blue);
        //Debug.DrawRay(TR_inner, Vector3.right, Color.blue);
    }

    public float DistanceAndAngleToGround_Moving(Vector2 dir, float dist, out Vector2 normal)
    {
        Vector2 origin = dir.x > 0 ? BR : BL;
        RaycastHit2D hit = Raycast(origin, dir, dist, groundLayer, Color.cyan);
        normal = Vector2.up;

        if (hit)
        {
            normal = hit.normal;
            Debug.DrawRay(origin, dir, Color.red, 4f);
            Debug.DrawRay(hit.point, hit.normal, Color.yellow, 4f);
        }
        return hit.distance;
    }

    public float DistanceAndAngleToGround_NonMoving(float yVelocity, out float angle)
    {
        //Find the position this object would be when on the ground. 

        RaycastHit2D left = Raycast(BL, Vector2.down, yVelocity, groundLayer, Color.cyan);
        RaycastHit2D right = Raycast(BR, Vector2.down, yVelocity, groundLayer, Color.magenta);

        //if (left)
        //    Debug.DrawRay(left.point, left.normal, Color.cyan, 0.1f);
        //else if (right)
        //    Debug.DrawRay(right.point, right.normal, Color.magenta, 0.1f);



        if (!left && !right) //Neither hits
        {
            angle = 0f;
            return 0f;
        }
        else if (left && !right) //Left hits
        {
            angle = GetSlopeAngle(left.normal);
            return left.distance;
        }
        else if (!left && right) //Right hits
        {
            //Debug.DrawRay(BR, right.normal, Color.grey, 0.1f);
            //Debug.DrawRay(BR + Vector2.down * yVelocity, right.normal, Color.white, 0.1f);

            //Debug.DrawRay(BR + Vector2.down * right.distance, right.normal, Color.magenta, 0.1f);
            //Debug.DrawRay(right.point, Vector3.right, Color.grey, 0.1f);

            angle = GetSlopeAngle(right.normal);
            return right.distance;
        }
        else //Both hits
        {
            if (left.distance < right.distance)
            {
                angle = GetSlopeAngle(left.normal);
                return left.distance;
            }
            else
            {
                angle = GetSlopeAngle(right.normal);
                return right.distance;
            }
        }
    }

    public float CheckForCeilingSideNudge(float yVelocity)
    {
        //If 
        bool L_inner = Raycast(TL_inner, Vector2.up, yVelocity, groundLayer, Color.cyan);
        bool L_outer = Raycast(TL_outer, Vector2.up, yVelocity, groundLayer, Color.cyan);
        bool R_inner = Raycast(TR_inner, Vector2.up, yVelocity, groundLayer, Color.magenta);
        bool R_outer = Raycast(TR_outer, Vector2.up, yVelocity, groundLayer, Color.magenta);

        bool canNudgeRight = L_outer && !L_inner;
        bool canNudgeLeft = R_outer && !R_inner;

        if (canNudgeLeft && !canNudgeRight)
        {
            return -sideNudgeDist;
        }
        else if (!canNudgeLeft && canNudgeRight)
        {
            return sideNudgeDist;
        }
        return 0f;
    }

    public int GetWallDirSign()
    {
        if (Raycast(TR_outer, Vector2.right, checkDist, groundLayer, Color.blue) ||
            Raycast(BR, Vector2.right, checkDist, groundLayer, Color.blue))
        {
            return 1;
        }
        else if (Raycast(TL_outer, Vector2.left, checkDist, groundLayer, Color.blue) ||
            Raycast(BL, Vector2.left, checkDist, groundLayer, Color.blue))
        {
            return -1;
        }
        return 0;
    }

    public bool HitsCeiling()
    {
        RaycastHit2D left = Raycast(TL_outer, Vector2.up, checkDist, groundLayer, Color.yellow);
        RaycastHit2D right = Raycast(TR_outer, Vector2.up, checkDist, groundLayer, Color.red);
        return left || right ? true : false;
    }
    #endregion

    #region Collision checks
    bool OnGroundCheck()
    {
        RaycastHit2D left = Raycast(BL, Vector2.down, checkDist, groundLayer, Color.yellow);
        RaycastHit2D right = Raycast(BR, Vector2.down, checkDist, groundLayer, Color.red);
        return left || right ? true : false;
    }
    #endregion

    #region Util

    RaycastHit2D Raycast(Vector2 origin, Vector2 dir, float dist, LayerMask mask, Color color)
    {
        Debug.DrawRay(origin, dir * dist, color);
        return Physics2D.Raycast(origin, dir, dist, mask);
    }

    float GetSlopeAngle(Vector2 slopeNormal)
    {
        return Vector2.Angle(slopeNormal, Vector2.up);
    }
    #endregion
}


//float direction = facingRight ? 1f : -1f;
//bot = Physics2D.Raycast(new Vector2(direction * extentX, -extentY), Vector3.right * direction, CheckDistance);
//top = Physics2D.Raycast(new Vector2(direction * extentX, extentY), Vector3.right * direction, CheckDistance);
//Debug.DrawRay(new Vector2(direction * extentX, -extentY), Vector3.right * direction * CheckDistance, Color.green);
//Debug.DrawRay(new Vector2(direction * extentX,  extentY), Vector3.right * direction * CheckDistance, Color.blue);

//public bool IsAgainstWall(int facingSign)
//{
//    RaycastHit2D bot = Raycast(BR, Vector2.right * facingSign, checkDist, Color.green);
//    RaycastHit2D top = Raycast(TR_outer, Vector2.right * facingSign, checkDist, Color.blue);

//    return (bot && top) ? true : false;
//}