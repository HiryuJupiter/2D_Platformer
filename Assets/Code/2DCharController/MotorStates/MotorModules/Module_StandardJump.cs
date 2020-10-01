using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Jump Burst", menuName = "Jump Modules/Burst")]

public class Module_StandardJump : ModuleBase
{
    const float MaxJumpQueueDuration = 0.05f;

    public Module_StandardJump(Player2DController_Motor motor) : base(motor) { }

    #region Public 
    public override void ModuleEntry()
    {
        base.ModuleEntry();
        if (status.jumpQueueTimer > 0f)
        {
            OnJumpBtnDown();
        }
    }

    public override void TickUpdate()
    {
        TickTimers();

        if (GameInput.JumpBtnDown && status.canJump) // && !isJumping for onGround
        {
            OnJumpBtnDown();
        }

        if (GameInput.JumpBtn)
        {
            OnJumpBtnHold();
        }

        if (GameInput.JumpBtnUp)
        {
            OnJumpBtnUp();
        }
    }

    public override void TickFixedUpdate()
    {
        base.TickFixedUpdate();
        if (status.justLanded)
        {
            status.isJumping = false;
            status.coyoteTimer = -1f;
        }
    }
    #endregion

    void OnJumpBtnDown()
    {
        status.isJumping = true;
        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;

        status.currentVelocity.y = settings.MaxJumpForce;
    }

    void OnJumpBtnHold()
    {
        status.jumpQueueTimer = MaxJumpQueueDuration;
    }

    void OnJumpBtnUp()
    {
        if (status.currentVelocity.y > settings.MinJumpForce)
        {
            status.currentVelocity.y = settings.MinJumpForce;
        }
    }

    void TickTimers()
    {
        if (status.coyoteTimer > 0f)
        {
            status.coyoteTimer -= Time.deltaTime;
        }

        if (status.jumpQueueTimer > 0f)
        {
            status.jumpQueueTimer -= Time.deltaTime;
        }
    }
}

//public override void TickUpdate()
//{
//    if (status.currentVelocity.y > 0)
//    {
//        status.currentVelocity.y = 0;
//    }
//}
