using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MotorRaycaster))]
public class Player2DMotor : MonoBehaviour
{
    //Classes and inspector components
    Rigidbody2D rb; //The rigidbody component
    Player2DFeedbacks Feedbacks; //The script responsible for display visuals, changing animations
    GameSettings settings;

    //States
    MotorStates currentStateType; // The current state represented by a custom enum type
    MotorStateBase currentStateClass; //The current active state class
    Dictionary<MotorStates, MotorStateBase> stateClassLookup; // The lookup table that matches an enum state to the corresponding class.

    public MotorStatus status { get; private set; } //A custom class that stores all status (e.g. isOnGround) in one place
    public MotorRaycaster raycaster { get; private set; } //The component that does the raycast checks. 


    #region Public 
    public void SwitchToNewState(MotorStates newStateType) //This method tells the class to change to a state
    {
        if (currentStateType != newStateType) //Only change the state when we're not already there
        {
            currentStateType = newStateType;

            currentStateClass.StateExit(); //Tell the current state we're exiting, so it can perform clean ups.
            currentStateClass = stateClassLookup[newStateType];
            currentStateClass.StateEntry(); //Tell the new state we're entering, so it can perform initializations.
        }
    }

    public void ForceNudge(Vector2 nudge) => rb.position += nudge; //This is used for edge cases where we want to force a movement on the player.

    public void DamagePlayer(Vector2 enemyPos) //Damage the player, the player will cache the enemy position and then go to hurt-state, which will handle what to do with the event.
    {
        status.lastEnemyPosition = enemyPos;
        SwitchToNewState(MotorStates.Hurt);
    }
    #endregion


    #region MonoBehiavor
    void Awake()
    {
        //Reference
        rb = GetComponent<Rigidbody2D>();
        raycaster = GetComponent<MotorRaycaster>();
        Feedbacks = GetComponentInChildren<Player2DFeedbacks>();

        //Initialize various variables
        status = new MotorStatus();
        stateClassLookup = new Dictionary<MotorStates, MotorStateBase> //Store all the FSM classes using their common base class
        {
            {MotorStates.OnGround,  new MotorState_MoveOnGround(this, Feedbacks)},
            {MotorStates.Aerial,    new MotorState_Aerial(this, Feedbacks)},
            {MotorStates.WallClimb, new MotorState_WallClimb(this, Feedbacks)},
            {MotorStates.Hurt,      new MotorState_Hurt(this, Feedbacks)},
        };

        currentStateType = MotorStates.OnGround;
        currentStateClass = stateClassLookup[currentStateType];
    }

    void Start()
    {
        settings = GameSettings.instance; //Reference static singletons
    }

    void Update()
    {
        currentStateClass?.TickUpdate(); //Tell the non-monobehavior state classes we want to update
    }

    void FixedUpdate()
    {
        status.CacheCurrentValuesToOld(); //Cache the values in the previous frame before we update them
        raycaster.UpdateOriginPoints(); //We use a specialist raycaster class to do all the raycasting logic
        CacheStatusCalculations(); //Certain calculations can be done once and cached for all scripts to use later.

        currentStateClass?.TickFixedUpdate(); 

        rb.velocity = status.currentVelocity; //After all velocity calculations are finalized, we assign it to the rigidbody
    }
    #endregion

    #region Pre-calculations
    void CacheStatusCalculations()
    {
        //Certain calculations can be done once and cached for all scripts to use later.
        status.isOnGround = raycaster.IsOnGround; 
        status.moveInputSign = MathsUtil.SignAllowingZero(GameInput.MoveX);
        status.velocityXSign = MathsUtil.SignAllowingZero(status.currentVelocity.x);
    }
    #endregion

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 20), "Current State: " + currentStateType); 

        GUI.Label(new Rect(20, 60, 290, 20), "=== GROUND MOVE === ");
        GUI.Label(new Rect(20, 80, 290, 20), "OnGround: " + status.isOnGround);
        GUI.Label(new Rect(20, 100, 290, 20), "onGroundPrevious: " + status.isOnGroundPrevious);
        GUI.Label(new Rect(20, 120, 290, 20), "GameInput.MoveX: " + GameInput.MoveX);
        GUI.Label(new Rect(20, 140, 290, 20), "movingSign: " + status.moveInputSign);
        GUI.Label(new Rect(20, 160, 290, 20), "isMoving: " + status.isMoving);
        GUI.Label(new Rect(20, 180, 290, 20), "targetVelocity: " + status.currentVelocity);


        GUI.Label(new Rect(200, 0, 290, 20), "=== JUMPING === ");
        GUI.Label(new Rect(200, 20, 290, 20), "coyoteTimer: " + status.coyoteTimer);
        GUI.Label(new Rect(200, 40, 290, 20), "jumpQueueTimer: " + status.jumpQueueTimer);
        GUI.Label(new Rect(200, 60, 290, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);
        GUI.Label(new Rect(200, 80, 290, 20), "jumping: " + status.isJumping);

        //GUI.Label(new Rect(300, 120,		290, 20), "testLocation: " + testLocation);

        GUI.Label(new Rect(400, 0, 290, 20), "=== SLOPE === ");
        GUI.Label(new Rect(400, 20, 290, 20), "decending: " + status.descendingSlope);
        GUI.Label(new Rect(400, 40, 290, 20), "climbingSlope: " + status.climbingSlope);
        GUI.Label(new Rect(400, 60, 290, 20), "slopeAngle: " + status.slopeAngle);

        GUI.Label(new Rect(600, 0, 290, 20), "=== WALL CLIMB === ");
        GUI.Label(new Rect(600, 20, 290, 20), "wallSign: " + status.wallSign);
        GUI.Label(new Rect(600, 40, 290, 20), "wallStickTimer: " + status.wallStickTimer);
        GUI.Label(new Rect(600, 60, 290, 20), "isWallSliding: " + status.isWallSliding);
    }
}
