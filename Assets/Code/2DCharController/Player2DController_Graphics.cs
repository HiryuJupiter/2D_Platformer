using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class Player2DController_Graphics : MonoBehaviour
{
    bool facingRight;
    SpriteRenderer spriteRenderer;

    int facingSign;


    #region Mono
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        FaceRight();
    }

    void Start()
    {

    }
    void Update()
    {
        UpdateFacing();
    }
    #endregion

    #region Public
    public void UpdateFacing()
    {
        if (GameInput.MoveX > 0.1f)
        {
            facingSign = 1;
        }
        else if (GameInput.MoveX < -0.1f)
        {
            facingSign = -1;
        }

        SetFacing(GameInput.MoveX);
    }

    

    public void SetToRed ()
    {
        //spriteRenderer.color = Color.red;
    }

    public void SetToWhite()
    {
        //spriteRenderer.color = Color.white;
    }

    public void SetToBlue()
    {
        //spriteRenderer.color = Color.blue;
    }
    #endregion

    #region Facing
    void SetFacing(float moveX)
    {
        if ((moveX > 0.1f && !facingRight) ||
            (moveX < -0.1f && facingRight))
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

    }
    void FaceRight ()
    {
        facingRight = true;
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x);
        transform.localScale = theScale;
    }
    #endregion
}