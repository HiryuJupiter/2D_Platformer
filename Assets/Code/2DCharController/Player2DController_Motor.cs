using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;

/* === NOTES ===
 * Jumping and OnGround are two different situations that doesn't always overlap. After pressing jump, you're still onground for a few frames due to the raycast distance. You can also walk off a platform that cause you to become !onGround and !jumping.
 * For slope movement calculations, use the maximum moveSpeed instead of currentVelocity, since the currentVelocity.x is modified and reduced by the slope calculation and then by the SmoothDamp, which will make the player climb slope slower than it should be.
 */

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(Player2DRaycaster))]
[RequireComponent(typeof(JumpModule))]
public class Player2DController_Motor : MonoBehaviour
{
    //Public
    public JumpModule jumpModule;
    [HideInInspector] public Player2DRaycaster raycaster;
    [HideInInspector] public MotorStatus status;


    //Private serialized
    [Header("Setting")]
    [SerializeField] bool stickyGround = true;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] int maxSlopeAngle = 70;


    [Header("State classes")]
    [SerializeField] MotorModuleBase state_MoveOnGround;
    [SerializeField] MotorModuleBase state_Aerial;
    [SerializeField] MotorModuleBase state_WallClimb;

    //Class & components
    Rigidbody2D rb;

    //States
    MotorStates currentStateType;
    MotorModuleBase currentStateClass;

    //Status

    //Cache
    Dictionary<MotorStates, MotorModuleBase> stateLookUp;
    float decendSlopeMaxCheckDist;

    const float SkinWidth = 0.005f;


    #region Property
    public float MoveSpeed => moveSpeed;
    #endregion

    #region MonoBehiavor
    void Awake()
    {
        //Cache
        //Max raycast distance for down-slope calculations, based on max slope level and max move Speed.

        status = new MotorStatus();

        //Reference
        rb = GetComponent<Rigidbody2D>();
        raycaster = GetComponent<Player2DRaycaster>();

        //Initialize state
        jumpModule.Initialize(this);
        state_MoveOnGround.Initialize(this);
        state_Aerial.Initialize(this);
        state_WallClimb.Initialize(this);

        stateLookUp = new Dictionary<MotorStates, MotorModuleBase>
        {
            {MotorStates.OnGround,    state_MoveOnGround},
            {MotorStates.Aerial,      state_Aerial},
            {MotorStates.WallClimb,   state_WallClimb}
        };

        currentStateType = MotorStates.OnGround;
        currentStateClass = stateLookUp[currentStateType];

        //Cache
        decendSlopeMaxCheckDist = moveSpeed * Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad);

    }

    void FixedUpdate()
    {
        raycaster.UpdateOriginPoints();
        UpdateFacingSign();
        PhysicsCheck();
        currentStateClass.OnFixedUpdate();


        if (stickyGround && !status.isJumping)
        {
            StickToSlope();
        }

        rb.velocity = status.currentVelocity;
        status.CacheCurrentValuesToOld();


    }
    #endregion

    #region Public
    public void GoToState (MotorStates newState)
    {
        if (currentStateType != newState)
        {
            currentStateType = newState;

            currentStateClass.StateExit();
            currentStateClass = stateLookUp[newState];
            currentStateClass.StateEntry();
        }
    }
    #endregion

    #region Pre-check
    void PhysicsCheck()
    {
        status.isOnGround = raycaster.IsOnGround;

        if (status.isMovingUp)
        {
            NudgeAwayFromCeilingEdge();
            CheckForCeilingHit();
        }
    }

    void NudgeAwayFromCeilingEdge()
    {
        float nudgeX = raycaster.CheckForCeilingSideNudge(status.currentVelocity.y * Time.deltaTime);
        if (nudgeX != 0f)
        {
            Vector3 p = rb.position;
            p.x += nudgeX;
            rb.position = p;
        }
    }

    void CheckForCeilingHit()
    {
        if (raycaster.HitsCeiling)
        {
            jumpModule.NormalJump_OnButtonUp();
            status.currentVelocity.y = 0f;
            status.isJumping = false;
        }
    }
    #endregion

    #region Slope handling
    void StickToSlope()
    {
        status.climbingSlope = false;
        status.descendingSlope = false;

        if (status.isMoving)
        {
            //Ascending
            Vector2 frontfoot = status.movingSign > 0 ? raycaster.BR : raycaster.BL;
            StickToAscendingSlope(frontfoot);

            //Only allow decending slope when not currently ascending.
            if (!status.climbingSlope)
            {
                //Descending
                Vector2 backfoot = status.movingSign > 0 ? raycaster.BL : raycaster.BR;
                StickToDecendingSlope(backfoot);

                //This prevents "car-flying-over-ramp" effect when going over a slope.
                if (!status.descendingSlope && !status.isOnGround)
                {
                    //Stick to ground
                    if (status.currentVelocity.y > 0f)
                    {
                        status.currentVelocity.y = 0f;
                    }
                }
            }
        }
        else
        {
            if (status.isOnGround)
            {
                //Don't let player slide down a slope by gravity.
                status.currentVelocity.y = 0f;
            }
        }
    }

    void StickToAscendingSlope(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.right * status.movingSign, Mathf.Abs(status.currentVelocity.x) * Time.deltaTime, groundLayer);

        //Debug.DrawRay(origin, new Vector3(currentVelocity.x * Time.deltaTime, 0f, 0f), Color.cyan);

        if (hit)
        {
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            if (slopeAngle != 0 && slopeAngle < maxSlopeAngle)
            {
                status.climbingSlope = true;
                Vector3 newVelocity = Vector3.zero;
                float gapDist = 0f;
                //If there is space between you and the slope, then move right up against it.
                if (slopeAngle != status.slopeAngleOld) //For optimization, only do once per slope
                {
                    gapDist = hit.distance - SkinWidth;
                    newVelocity.x = gapDist / Time.deltaTime * status.movingSign;
                }
                //Take the full VelocityX, minus the gap distance, then use the remaining velocity X...
                //...to calculate slope climbing. 
                float climbDistance = moveSpeed - gapDist; //climbDistance is also the hypotenues
                float displaceX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDistance * status.movingSign;
                float displaceY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDistance;
                newVelocity.x += displaceX;
                newVelocity.y = displaceY;

                status.currentVelocity = newVelocity;
            }
        }
    }

    void StickToDecendingSlope(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, decendSlopeMaxCheckDist * Time.deltaTime, groundLayer);
        //Debug.DrawRay(origin, Vector3.down * decendSlopeCheckDist * Time.deltaTime, Color.red, 0.5f);

        if (hit)
        {
            //Debug.DrawRay(hit.point, hit.normal, Color.magenta, 0.5f);
            status.slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            //If the slope is less than maxSlope angle
            if (status.slopeAngle != 0 && status.slopeAngle < maxSlopeAngle)
            {
                //See if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
                if (Mathf.Sign(hit.normal.x) == status.movingSign)
                {
                    status.descendingSlope = true;
                    //Check if we are standing close enough to the platform to begin decend calculation. 
                    //float descendableRange = decendSlopeCheckDist ;
                    //if (hit.distance - SkinWidth < descendableRange)
                    {
                        //Specify the decend amount
                        //Btw we're using max move speed (moveSpeed) instead of currentVelocity.x because it is reduced by smoothdamp.
                        status.currentVelocity.x = Mathf.Cos(status.slopeAngle * Mathf.Deg2Rad) * moveSpeed * status.movingSign;
                        status.currentVelocity.y = -Mathf.Sin(status.slopeAngle * Mathf.Deg2Rad) * moveSpeed;
                        //currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
                        if (status.slopeAngle != status.slopeAngleOld)
                        {
                            //Make the player move towards the slop if it is hovering above it
                            //We use slopAngleOld for performance optimization
                            status.currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
                        }
                    }
                }
            }
        }
    }
    #endregion


    #region Util
    void UpdateFacingSign()
    {
        if (GameInput.MoveX > 0.1f)
        {
            status.movingSign = 1;
        }
        else if (GameInput.MoveX < -0.1f)
        {
            status.movingSign = -1;
        }
        else
        {
            status.movingSign = 0;
        }
    }
    #endregion

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 20), "Current State: " + currentStateType); 

        GUI.Label(new Rect(20, 60, 290, 20), "=== GROUND MOVE === ");
        GUI.Label(new Rect(20, 80, 290, 20), "OnGround: " + status.isOnGround);
        GUI.Label(new Rect(20, 100, 290, 20), "onGroundPrevious: " + status.isOnGroundPrevious);
        GUI.Label(new Rect(20, 120, 290, 20), "GameInput.MoveX: " + GameInput.MoveX);
        GUI.Label(new Rect(20, 140, 290, 20), "movingSign: " + status.movingSign);
        GUI.Label(new Rect(20, 160, 290, 20), "targetVelocity: " + status.currentVelocity);

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
    }
}

public class MotorStatus
{
    public bool isOnGround;
    public bool isOnGroundPrevious;
    public bool isJumping;
    public int movingSign;

    public float coyoteTimer;
    public float jumpQueueTimer;
        
    public int wallSign;

    public bool descendingSlope;
    public bool climbingSlope;
    
    public float slopeAngle;
    public float slopeAngleOld;


    public Vector3 currentVelocity;

    public bool isFalling => currentVelocity.y < 0f;
    public bool isMovingUp => currentVelocity.y > 0f;
    public bool isMoving => movingSign != 0;

    public void CacheCurrentValuesToOld ()
    {
        isOnGroundPrevious = isOnGround;
        slopeAngleOld = slopeAngle;
    }
}