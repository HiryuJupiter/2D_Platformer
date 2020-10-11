using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MotorRaycaster))]
public class Player2DMotor : MonoBehaviour
{
    //Class and components
    
    Rigidbody2D rb;
    Player2DFeedbacks Feedbacks;
    GameSettings settings;

    //States
    MotorStates currentStateType;
    MotorStateBase currentStateClass;
    Dictionary<MotorStates, MotorStateBase> stateClassLookup;

    public MotorStatus status { get; private set; }
    public MotorRaycaster raycaster { get; private set; }


    #region Public
    public void SwitchToNewState(MotorStates newStateType)
    {
        if (currentStateType != newStateType)
        {
            currentStateType = newStateType;

            currentStateClass.StateExit();
            currentStateClass = stateClassLookup[newStateType];
            currentStateClass.StateEntry();
        }
    }
    #endregion

    #region MonoBehiavor
    void Awake()
    {
        
        //Reference
        rb = GetComponent<Rigidbody2D>();
        raycaster = GetComponent<MotorRaycaster>();
        Feedbacks = GetComponentInChildren<Player2DFeedbacks>();

        //Initialize
        status = new MotorStatus();
        stateClassLookup = new Dictionary<MotorStates, MotorStateBase>
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
        settings = GameSettings.instance;
    }

    void Update()
    {
        currentStateClass?.TickUpdate();
    }

    void FixedUpdate()
    {
        status.CacheCurrentValuesToOld();
        raycaster.UpdateOriginPoints();
        CacheStatusCalculations();

        currentStateClass?.TickFixedUpdate();

        rb.velocity = status.currentVelocity;
    }
    #endregion

    #region Public 
    public void ForceNudge(Vector2 nudge) => rb.position += nudge;

    public void DamagePlayer(Vector2 enemyPos)
    {
        status.lastEnemyPosition = enemyPos;
        SwitchToNewState(MotorStates.Hurt);
    }
    #endregion

    #region Pre-calculations
    void CacheStatusCalculations()
    {
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
