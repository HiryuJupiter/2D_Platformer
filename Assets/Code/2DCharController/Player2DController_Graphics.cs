using UnityEngine;
using System.Collections;


public class Player2DController_Graphics : MonoBehaviour
{
    public bool facingRight { get; private set; } = true;

    #region Mono
    void Start()
    {

    }

    void Update()
    {

    }
    #endregion


    #region Public
    public void SetFacing(bool faceRight)
    {
        facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void SetToRed ()
    {

    }

    public void SetToWhite()
    {

    }

    public void SetToBlue()
    {

    }
    #endregion
}