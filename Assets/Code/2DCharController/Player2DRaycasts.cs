using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Player2DRaycasts : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;

    //Offsets
    Vector2 offset_BL; //Offset bottom left
    Vector2 offset_BR; //Offset bottom right
    Vector2 offset_TL; //Offset top left
    Vector2 offset_TR; //Offset top right

    //Const
    const float CheckDistance = 0.06f;

    #region Properties
    public bool IsOnGround => OnGroundCheck();
    public bool IsAgainstCeiling => CheckIsAgainstCeiling();
    public bool IsAgainstLeft => IsAgainstSide(true);
    public bool IsAgainstRight => IsAgainstSide(false);

    //The final world positions (not offsets)
    public Vector2 BL { get; private set; }
    public Vector2 BR { get; private set; }
    public Vector2 TL { get; private set; }
    public Vector2 TR { get; private set; }
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
        offset_TL = new Vector2(-x, y);
        offset_TR = new Vector2(x, y);
    }
    #endregion

    #region Public
    public void UpdateOriginPoints()
    {
        BL = (Vector2)transform.position + offset_BL;
        BR = (Vector2)transform.position + offset_BR;
        TL = (Vector2)transform.position + offset_TL;
        TR = (Vector2)transform.position + offset_TR;
    }
    #endregion

    #region Collision checks
    bool OnGroundCheck()
    {
        RaycastHit2D hit_BL_down = Physics2D.Raycast(BL, -Vector3.up, CheckDistance, groundLayer);
        RaycastHit2D hit_BR_down = Physics2D.Raycast(BR, -Vector3.up, CheckDistance, groundLayer);
        Debug.DrawRay(BL, Vector3.down * CheckDistance, Color.yellow);
        Debug.DrawRay(BR, Vector3.down * CheckDistance, Color.red);
        return hit_BL_down || hit_BR_down ? true : false;
    }

    bool CheckIsAgainstCeiling()
    {
        RaycastHit2D hit_TL_up = Physics2D.Raycast(TL, Vector3.up, CheckDistance, groundLayer);
        RaycastHit2D hit_TR_up = Physics2D.Raycast(TR, Vector3.up, CheckDistance, groundLayer);
        Debug.DrawRay(TL, Vector3.up * CheckDistance, Color.yellow);
        Debug.DrawRay(TR, Vector3.up * CheckDistance, Color.red);
        return hit_TL_up || hit_TR_up ? true : false;
    }

    bool IsAgainstSide(bool facingRight)
    {
        RaycastHit2D bot;
        RaycastHit2D top;

        if (facingRight)
        {
            bot = Physics2D.Raycast(BR, Vector3.right, CheckDistance);
            top = Physics2D.Raycast(TR, Vector3.right, CheckDistance);
            Debug.DrawRay(BR, Vector3.right * CheckDistance, Color.green);
            Debug.DrawRay(TR, Vector3.right * CheckDistance, Color.blue);
        }
        else
        {
            bot = Physics2D.Raycast(BL, Vector3.left, CheckDistance);
            top = Physics2D.Raycast(TL, Vector3.left, CheckDistance);
            Debug.DrawRay(BL, Vector3.left * CheckDistance, Color.green);
            Debug.DrawRay(TL, Vector3.left * CheckDistance, Color.blue);
        }

        return (bot && top) ? true : false;
    }
    #endregion
}


//float direction = facingRight ? 1f : -1f;
//bot = Physics2D.Raycast(new Vector2(direction * extentX, -extentY), Vector3.right * direction, CheckDistance);
//top = Physics2D.Raycast(new Vector2(direction * extentX, extentY), Vector3.right * direction, CheckDistance);
//Debug.DrawRay(new Vector2(direction * extentX, -extentY), Vector3.right * direction * CheckDistance, Color.green);
//Debug.DrawRay(new Vector2(direction * extentX,  extentY), Vector3.right * direction * CheckDistance, Color.blue);
