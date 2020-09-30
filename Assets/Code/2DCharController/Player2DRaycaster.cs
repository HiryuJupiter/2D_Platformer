using UnityEngine;
using System.Collections;

/*
 What is side nudge? It's when the player controller hits the side of a platform 
above it by just a tiny bit, and the game automatically nudges the player out of the
way.
 */

[RequireComponent(typeof(Collider2D))]
public class Player2DRaycaster : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;

    //Cache
    LayerMask allLayers = ~0;

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


    public float DistanceToGround (float yVelocity)
    {
        //Find the position this object would be when on the ground. 

        RaycastHit2D left = Raycast(BL, Vector2.down, yVelocity, groundLayer, Color.cyan);
        RaycastHit2D right = Raycast(BR, Vector2.down, yVelocity, groundLayer, Color.magenta);

        float leftDist;
        float rightDist;

        if (left)
            leftDist = left.distance;
        if (right)
            rightDist = right.distance;

        if (!left && !right) //Neither hits
        {
            return 0f;
        }
        else if (left && !right) //Left hits
        {
            return left.distance;
        }
        else if (!left && right) //Right hits
        {
            return right.distance;
        }
        else //Both hits
        { 
            if (left.distance < right.distance)
            {
                return left.distance;
            }
            else
            {
                return right.distance;
            }
        }
    }

    public float CheckForCeilingSideNudge(float yVelocity)
    {
        //If 
        bool L_inner= Raycast(TL_inner, Vector2.up, yVelocity, groundLayer, Color.cyan);
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
        RaycastHit2D right;
        RaycastHit2D left;
        if (right = Raycast(TR_outer, Vector2.right, checkDist, groundLayer, Color.blue))
        {
            Debug.DrawRay(right.point, Vector3.up, Color.magenta, 1f);
            return 1;
        }
        else if (left = Raycast(TL_outer, Vector2.left, checkDist, groundLayer, Color.blue))
        {
            Debug.DrawRay(left.point, Vector3.up, Color.magenta, 1f);
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
    RaycastHit2D Raycast(Vector2 origin, Vector2 dir, float dist, Color color)
    {
        return Raycast(origin, dir, dist, allLayers, color);
    }

    RaycastHit2D Raycast(Vector2 origin, Vector2 dir, float dist, LayerMask mask, Color color)
    {
        Debug.DrawRay(origin, dir * dist, color);
        return Physics2D.Raycast(origin, dir, dist, mask);
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