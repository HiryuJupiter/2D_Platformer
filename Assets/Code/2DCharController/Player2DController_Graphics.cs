using UnityEngine;
using System.Collections;


public class Player2DController_Graphics : MonoBehaviour
{
    bool facingRight;
    SpriteRenderer spriteRenderer;

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

    }
    #endregion


    #region Public
    public void SetFacing(float moveX)
    {
        if ((moveX > 0.1f && !facingRight) ||
            (moveX < -0.1f && facingRight))
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
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

    #region Granular logic
    void FaceRight ()
    {
        facingRight = true;
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x);
        transform.localScale = theScale;
    }
    #endregion
}