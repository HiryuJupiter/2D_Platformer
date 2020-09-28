using UnityEngine;
using System.Collections;

/*=== TERMINOLOGIES ===
 * COYOTE TIME: After walking off a platform, the player has a brief moment where they can still jump.
 * JUMP QUEUE TIMER: The player can cache the jump command for a brief moment while midair. If it is cached right before landing, the player will automatically jump after landing. 
 */

[CreateAssetMenu(fileName = "MotorState_Aerial", menuName = "Motor States/Aerial")]
public class MotorState_Aerial : MotorModuleBase
{
    [SerializeField] LayerMask groundLayer;
    
    [Range(0.1f, 4f)] 
    [SerializeField] float steerSpeed = 4f; //15
    [SerializeField] float gravity = 80f;
    [SerializeField] float maxFallSpeed = -15f;

    //Status
    float moveXSmoothDampVelocity;

    const float MaxCoyoteDuration = 0.2f;
    const float MaxJumpQueueDuration = 0.05f;


    #region Public 
    public override void StateEntry()
    {
        UpdateCoyoteTimer();
    }

    public override void OnUpdate() 
    {
        TickTimers();
        JumpInputDetection();
        UpdateHorizontalMove();
    }

    public override void OnFixedUpdate()
    {
        UpdateHorizontalMove();
        ApplyGravity();
        GravityOvershootPrevention();
        if (status.isOnGround)
        {
            motor.GoToState(MotorStates.OnGround);
        }
    }

    public override void StateExit()
    {
        status.isJumping = false;
        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;
    }
    #endregion

    #region Move
    void UpdateCoyoteTimer()
    {
        if (!status.isOnGround && status.isOnGroundPrevious && !status.isJumping)
        {
            status.coyoteTimer = MaxCoyoteDuration;
        }
    }

    void UpdateHorizontalMove()
    {
        status.currentVelocity.x = Mathf.SmoothDamp(status.currentVelocity.x, GameInput.MoveX * motor.MoveSpeed, ref moveXSmoothDampVelocity, steerSpeed * Time.deltaTime);
    }
    #endregion


    #region Jumping logic
    void JumpInputDetection()
    {
        if (GameInput.JumpBtnDown && status.coyoteTimer > 0f) // && !isJumping for onGround
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

    void OnJumpBtnDown()
    {
        status.isJumping = true;
        status.jumpQueueTimer = -1f;
        status.coyoteTimer = -1f;

        jumpModule.NormalJump_OnButtonDown();
    }

    void OnJumpBtnHold()
    {
        status.jumpQueueTimer = MaxJumpQueueDuration;
    }

    void OnJumpBtnUp()
    {
        //isJumping = false;
        jumpModule.NormalJump_OnButtonUp();
    }
    #endregion

    #region Gravity
    void ApplyGravity()
    {
        if (!status.isOnGround)
        {
            status.currentVelocity.y -= gravity * Time.deltaTime;
            status.currentVelocity.y = Mathf.Clamp(status.currentVelocity.y, maxFallSpeed, status.currentVelocity.y);
        }
    }

    //Stops the player from sliding on slopes on the frame that they lands.
    void GravityOvershootPrevention()
    {
        if (status.isFalling && !status.isMoving)
        {
            //If the falling velocity is going below the ground, then reduce the velocity.
            float distance = raycaster.DistanceToGround(-status.currentVelocity.y * Time.deltaTime);
            if (distance > 0)
            {
                //Debug.DrawRay(raycaster.BR, Vector3.right, Color.yellow);
                //Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.green);

                RaycastHit2D right = Physics2D.Raycast(raycaster.BR, Vector2.down, -status.currentVelocity.y * Time.deltaTime, groundLayer);
                //Debug.DrawRay(right.point, Vector3.right, Color.magenta);

                status.currentVelocity.y = -distance; //THis is absolutely perfct in Non-interpolate. Interpolation does make the character slide upon landing, but this is by far the best option.
                                                      //Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.cyan);
            }
        }
    }
    #endregion

    #region Util
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
    #endregion
}