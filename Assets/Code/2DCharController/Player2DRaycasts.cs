using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Player2DRaycasts : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;

    //Offsets
    Vector2 offset_BL;
    Vector2 offset_BR;
    Vector2 offset_TL;
    Vector2 offset_TR;

    //Cache hit data
    RaycastHit2D hit_BL_down;
    RaycastHit2D hit_BR_down;

    //Const
    const float CheckDistance = 0.08f;

    #region Properties
    public bool OnGround => OnGroundCheck();
    public bool AgainstCeiling => IsAgainstCeiling();
    public bool AgainstLeft => IsAgainstSide(true);
    public bool AgainstRight => IsAgainstSide(false);
    public Vector2 GroundGradient => GetGroundGradient();

    Vector2 BL => (Vector2)transform.position + offset_BL;
    Vector2 BR => (Vector2)transform.position + offset_BR;
    Vector2 TL => (Vector2)transform.position + offset_TL;
    Vector2 TR => (Vector2)transform.position + offset_TR;
    #endregion

    #region MonoBehavior
    private void Awake()
    {
        //Initialize offset cache
        Bounds bounds = GetComponent<Collider2D>().bounds;
        float x = bounds.extents.x - CheckDistance / 2f;
        float y = bounds.extents.y - CheckDistance / 2f;
        offset_BL = new Vector2(-x, -y); 
        offset_BR = new Vector2(x, -y);
        offset_TL = new Vector2(-x, y);
        offset_TR = new Vector2(x, y);
    }
    #endregion

    #region Collision checks
    Vector2 GetGroundGradient ()
    {
        Vector2 p1 = hit_BL_down.point;
        Vector2 p2 = hit_BL_down.point;

        float x = p2.x - p1.x;
        float y = p2.y - p1.y;

        return new Vector2(y, -x);
    }

    bool OnGroundCheck()
    {
        hit_BL_down = Physics2D.Raycast(BL, -Vector3.up, CheckDistance, groundLayer);
        hit_BR_down = Physics2D.Raycast(BR, -Vector3.up, CheckDistance, groundLayer);
        Debug.DrawRay(BL, -Vector3.up * CheckDistance, Color.yellow);
        Debug.DrawRay(BR, -Vector3.up * CheckDistance, Color.red);
        return hit_BL_down || hit_BR_down ? true : false;
    }

    bool IsAgainstCeiling()
    {
        RaycastHit2D hit_TL_up = Physics2D.Raycast(TL, Vector3.up, CheckDistance, groundLayer);
        RaycastHit2D hit_TR_up = Physics2D.Raycast(TR, Vector3.up, CheckDistance, groundLayer);
        Debug.DrawRay(TL, Vector3.up * CheckDistance, Color.yellow);
        Debug.DrawRay(TR, Vector3.up * CheckDistance, Color.red);
        return hit_TL_up || hit_TR_up ? true : false;
    }

    bool IsAgainstSide(bool left)
    {
        RaycastHit2D bot;
        RaycastHit2D top;

        if (left)
        {
            bot = Physics2D.Raycast(BL, Vector3.left, CheckDistance);
            top = Physics2D.Raycast(TL, Vector3.left, CheckDistance);
            Debug.DrawRay(BL, Vector3.left * CheckDistance, Color.green);
            Debug.DrawRay(TL, Vector3.left * CheckDistance, Color.blue);
        }
        else
        {
            bot = Physics2D.Raycast(BR, Vector3.right, CheckDistance);
            top = Physics2D.Raycast(TR, Vector3.right, CheckDistance);
            Debug.DrawRay(BR, Vector3.right * CheckDistance, Color.green);
            Debug.DrawRay(TR, Vector3.right * CheckDistance, Color.blue);
        }

        return (bot && top) ? true : false;
    }
    #endregion
}