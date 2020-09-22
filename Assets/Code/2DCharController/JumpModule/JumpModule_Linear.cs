﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Jump Linear", menuName = "Jump Modules/Linear")]
public class JumpModule_Linear : JumpModule
{
    [SerializeField] float jumpForce = 22f;
    [SerializeField] float jumpMinDuration = 0.1f;
    [SerializeField] float jumpMaxDuration = 0.2f;

    float jumpTimer;
    bool isJumping;

    public override void StartApplyingJumpForce()
    {
        if (!isJumping)
            motor.StartCoroutine(ContinuouslyApplyJumpForce());
    }

    public override void StopApplyingJumpForce()
    {
        jumpTimer = float.MaxValue;
    }

    IEnumerator ContinuouslyApplyJumpForce ()
    {
        isJumping = true;
        jumpTimer = 0f;

        while (jumpTimer < jumpMinDuration || (GameInput.JumpBtn && jumpTimer < jumpMaxDuration))
        {
            motor.SetVelocityY(jumpForce);
            jumpTimer += Time.deltaTime;
            yield return null;
        }

        isJumping = false;
    }
}