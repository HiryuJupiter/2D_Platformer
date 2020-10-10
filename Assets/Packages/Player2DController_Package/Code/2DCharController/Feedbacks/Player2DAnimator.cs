using UnityEngine;
using System.Collections;

public class Player2DAnimator : MonoBehaviour
{
    //Component reference
    Animator animator;

    //Parameter ID
    int onGroundParamID;
    int isCrouchingParamID;
    int xVelocityParamID;
    int yVelocityParamID;
    int isHurtParamID;

    #region Mono
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        onGroundParamID = Animator.StringToHash("isOnGround");
        isCrouchingParamID = Animator.StringToHash("isCrouching");
        xVelocityParamID = Animator.StringToHash("HorizontalVelocity");
        yVelocityParamID = Animator.StringToHash("VerticalVelocity");
        isHurtParamID = Animator.StringToHash("isHurt");
    }
    #endregion
    public void SetOnGround(bool isTrue)
    {
        animator.SetBool(onGroundParamID, isTrue);
    }

    public void SetIsCrouch(bool isTrue)
    {
        animator.SetBool(isCrouchingParamID, isTrue);
    }

    public void SetXVelocity(float xVelocity)
    {
        animator.SetFloat(xVelocityParamID, xVelocity);
    }

    public void SetYVelocity(float yVelocity)
    {
        animator.SetFloat(yVelocityParamID, yVelocity);
    }

    public void SetIsHurt(bool isTrue)
    {
        animator.SetBool(isHurtParamID, isTrue);
    }
}