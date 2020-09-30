using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

/* === NOTES ===
 * Jumping and OnGround are two different situations that doesn't always overlap. After pressing jump, you're still onground for a few frames due to the raycast distance. You can also walk off a platform that cause you to become !onGround and !jumping.
 * For slope movement calculations, use the maximum moveSpeed instead of currentVelocity, since the currentVelocity.x is modified and reduced by the slope calculation and then by the SmoothDamp, which will make the player climb slope slower than it should be.
 */

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(Player2DRaycaster))]
[RequireComponent(typeof(Module_StandardJump))]
public class Player2DController_Motor : MonoBehaviour
{
    

    //States
    MotorStates currentStateType;
    MotorStateBase currentStateClass;
    Dictionary<MotorStates, MotorStateBase> states;

    public Player2DRaycaster Raycaster { get; private set; }
    public MotorStatus Status { get; private set; }
    public Rigidbody2D rb { get; private set; }

    #region MonoBehiavor
    void Awake()
    {
        //Reference
        rb = GetComponent<Rigidbody2D>();
        Raycaster = GetComponent<Player2DRaycaster>();

        //Initialize
        Status = new MotorStatus();
        states = new Dictionary<MotorStates, MotorStateBase>
        {
            {MotorStates.OnGround,    new MotorState_MoveOnGround(this)},
            {MotorStates.Aerial,      new MotorState_Aerial(this)},
            {MotorStates.WallClimb,   new MotorState_WallClimb(this)}
        };

        currentStateType = MotorStates.OnGround;
        currentStateClass = states[currentStateType];
        
    }

    private void Update()
    {
        currentStateClass?.TickUpdate();
    }

    void FixedUpdate()
    {
        Raycaster.UpdateOriginPoints();

        //Basic status checks
        CacheStatus();

        currentStateClass?.TickFixedUpdate();

        rb.velocity = Status.currentVelocity;
        Status.CacheCurrentValuesToOld();
    }
    #endregion

    #region Public
    public void SwitchToNewState (MotorStates newState)
    {
        if (currentStateType != newState)
        {
            currentStateType = newState;

            currentStateClass.StateExit();
            currentStateClass = states[newState];
            currentStateClass.StateEntry();
        }
    }
    #endregion

    #region Pre-check
    void CacheStatus()
    {
        CacheSigns();
        Status.isOnGround = Raycaster.IsOnGround;
    }

    void CacheSigns()
    {

        if (GameInput.MoveX > 0.1f)
        {
            Status.moveSign = 1;
        }
        else if (GameInput.MoveX < -0.1f)
        {
            Status.moveSign = -1;
        }
        else
        {
            Status.moveSign = 0;
        }
    }
    #endregion

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 20), "Current State: " + currentStateType); 

        GUI.Label(new Rect(20, 60, 290, 20), "=== GROUND MOVE === ");
        GUI.Label(new Rect(20, 80, 290, 20), "OnGround: " + Status.isOnGround);
        GUI.Label(new Rect(20, 100, 290, 20), "onGroundPrevious: " + Status.isOnGroundPrevious);
        GUI.Label(new Rect(20, 120, 290, 20), "GameInput.MoveX: " + GameInput.MoveX);
        GUI.Label(new Rect(20, 140, 290, 20), "movingSign: " + Status.moveSign);
        GUI.Label(new Rect(20, 160, 290, 20), "targetVelocity: " + Status.currentVelocity);

        GUI.Label(new Rect(200, 0, 290, 20), "=== JUMPING === ");
        GUI.Label(new Rect(200, 20, 290, 20), "coyoteTimer: " + Status.coyoteTimer);
        GUI.Label(new Rect(200, 40, 290, 20), "jumpQueueTimer: " + Status.jumpQueueTimer);
        GUI.Label(new Rect(200, 60, 290, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);
        GUI.Label(new Rect(200, 80, 290, 20), "jumping: " + Status.isJumping);

        //GUI.Label(new Rect(300, 120,		290, 20), "testLocation: " + testLocation);

        GUI.Label(new Rect(400, 0, 290, 20), "=== SLOPE === ");
        GUI.Label(new Rect(400, 20, 290, 20), "decending: " + Status.descendingSlope);
        GUI.Label(new Rect(400, 40, 290, 20), "climbingSlope: " + Status.climbingSlope);
        GUI.Label(new Rect(400, 60, 290, 20), "slopeAngle: " + Status.slopeAngle);

        GUI.Label(new Rect(600, 0, 290, 20), "=== WALL CLIMB === ");
        GUI.Label(new Rect(600, 20, 290, 20), "wallSign: " + Status.wallSign);
        GUI.Label(new Rect(600, 40, 290, 20), "wallStickTimer: " + Status.wallStickTimer);
        GUI.Label(new Rect(600, 60, 290, 20), "isWallSliding: " + Status.isWallSliding);
    }
}
