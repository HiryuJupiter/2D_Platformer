using UnityEngine;
using System.Collections;
/*
 mechanim is not appliable to frame based animation, because you can just tell it to 
Instead of setfloat, just animator.Play(

Using setFloat you're offloading logic to another system that handles the transition, 
but you're probably also handling logic handling in code, it's much easier to do everything in code in one place

coolhotkey: alt + arrow to shift lines


 */
public class Player2DAnimator : MonoBehaviour
{
    //Component reference
    Animator animator;
    int currentState;

    //Parameter ID for states
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

    public void SetFloat_XVelocity(float xVelocity)
    {
        animator.SetFloat(xVelocityParamID, xVelocity);
    }

    public void SetFloat_YVelocity(float yVelocity)
    {
        animator.SetFloat(yVelocityParamID, yVelocity);
    }

    void ChangeAnimationState (int newState)
    {
        if (currentState != newState)
        {
            animator.Play(newState);
            currentState = newState;
        }
    }

    float GetCurrentAnimationDuration ()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    IEnumerator DelayedTransitionToAnimation (float delay, int newAnimationParamID)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(newAnimationParamID);
    }
}