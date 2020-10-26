using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(Player2DAnimator))]
public class Player2DFeedbacks : MonoBehaviour
{
    //Component reference
    public Player2DAnimator Animator { get; private set; }

    //Facing
    bool facingRight;
    Vector3 faceRightScale, faceLeftScale;

    void Awake()
    {
        //Reference
        Animator = GetComponent<Player2DAnimator>();

        //Initialization
        faceRightScale = transform.localScale;
        faceLeftScale = faceRightScale;
        faceLeftScale.x *= -1f;

        SetFacingToRight(true);
    }

    public void SetFacingBasedOnInput ()
    {
        //Change facing based on the keys pressed
        if (GameInput.PressedRight)
        {
            SetFacingToRight(true);
        }
        else if (GameInput.PressedLeft)
        {
            SetFacingToRight(false);
        }
    }

    public void SetFacingToRight(bool right)
    {
        //Execute the facing change
        if (right && !facingRight)
        {
            facingRight = true;
            transform.localScale = faceRightScale;
        }
        else if (!right && facingRight)
        {
            facingRight = false;
            transform.localScale = faceLeftScale;
        }
    }
}
