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
        Animator = GetComponent<Player2DAnimator>();
        faceRightScale = transform.localScale;
        faceLeftScale = faceRightScale;
        faceLeftScale.x *= -1f;

        SetFacingToRight(true);
    }

    public void SetFacingBasedOnInput ()
    {
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

public enum PlayerAnimations { Idle, Run, Crawl, Jump}