using UnityEngine;
using System.Collections;

public class Player2DAnimator : MonoBehaviour
{
    //Component reference
    Animator animator;
    int currentState;

    //Parameter ID for animator
    int crouchParamID;
    int onGroundParamID;
    int aerialParamID;
    int hurtParamID;

    int xVelocityParamID;
    int yVelocityParamID;

    #region Mono
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        //Param ID: Floats
        xVelocityParamID = Animator.StringToHash("HorizontalVelocity");
        yVelocityParamID = Animator.StringToHash("VerticalVelocity");

        //Param ID: Booleans
        crouchParamID   = Animator.StringToHash("Crouch");
        onGroundParamID = Animator.StringToHash("OnGround");
        aerialParamID   = Animator.StringToHash("Aerial");
        hurtParamID     = Animator.StringToHash("Hurt");
    }
    #endregion

    //Play the corresponding animation inside the animator
    public void PlayOnGround ()
    {
        ChangeAnimationState(onGroundParamID);
    }

    public void PlayAerial()
    {
        ChangeAnimationState(aerialParamID);
    }

    public void PlayHurt()
    {
        ChangeAnimationState(hurtParamID);
    }

    public void PlayCrouch()
    {
        ChangeAnimationState(crouchParamID);
    }

    //Set float values inside the animator in order to do blending
    public void SetFloat_XVelocity(float xVelocity)
    {
        animator.SetFloat(xVelocityParamID, xVelocity);
    }

    public void SetFloat_YVelocity(float yVelocity)
    {
        animator.SetFloat(yVelocityParamID, yVelocity);
    }

    //Tell the animation to play the animation
    void ChangeAnimationState (int newState)
    {
        if (currentState != newState)
        {
            animator.Play(newState);
            currentState = newState;
        }
    }

    //The the current animation duration
    float GetCurrentAnimationDuration ()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
}