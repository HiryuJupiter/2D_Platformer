using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "MotorState_MoveOnGround", menuName = "Motor States/Move On Ground")]
public class MotorState_MoveOnGround : MotorModuleBase
{
    [SerializeField] LayerMask groundLayer;

    [Header("Movement")]
    //[Range(0, 1)]	[SerializeField] float crouchMoveSpeed = .36f;
    [Range(0.1f, 4f)] [SerializeField] float steerSpeed = 1f; //50f



    //Status
    float moveXSmoothDampVelocity;

    //Cache
    const float MaxCoyoteDuration = 0.2f;

    #region Public 
    public override void Initialize(Player2DController_Motor motor)
    {
        base.Initialize(motor);
    }
    //public MotorState_OnGround(Player2DController_Motor motor) : base (motor)
    //{
    //    decendSlopeMaxCheckDist = moveSpeed * Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad);
    //}

    public override void StateEntry()
    {
        if (status.jumpQueueTimer > 0f) //Automatically jumps if player has queued a jump command.
        {
            jumpModule.NormalJump_OnButtonDown();
        }
        else if (status.isFalling)
        {
            status.currentVelocity.y = 0;
        }
    }


    public override void OnFixedUpdate()
    {
        UpdateHorizontalMove();
        

        //Transitions
        if (!status.isOnGround)
        {
            motor.GoToState(MotorStates.Aerial);
        }
    }

    public override void StateExit()
    {
    }
    #endregion

    #region Movement
    void UpdateHorizontalMove()
    {
        status.currentVelocity.x = Mathf.SmoothDamp(status.currentVelocity.x, GameInput.MoveX * motor.MoveSpeed, ref moveXSmoothDampVelocity, steerSpeed * Time.deltaTime);
    }
    #endregion


}