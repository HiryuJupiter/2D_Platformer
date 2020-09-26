using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

/*Coyote time: A short time period allows the player to jump after walking off a platform.
Jump queue timer: Allows the player to "cache" the jump command when pressing jump right before land on ground.
Jumping and OnGround are two different situations that doesn't always overlap. After pressing jump, you're still onground for a few frames
due to the raycast distance. And you can also walk off a platform that cause you to become not-on-ground and no-jumping.
 */

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(Player2DRaycasts))]
[RequireComponent(typeof(JumpModule))]
public class Player2DController_Motor : MonoBehaviour
{
	//[Header("References")]
	//[SerializeField] Collider2D collider_standing;

	[Header("Movement")]
	[SerializeField] float moveSpeed = 10f;
	//[Range(0, 1)]	[SerializeField] float crouchMoveSpeed = .36f;
	[Range(0.1f, 4f)] [SerializeField] float steerSpeedOnGround = 1f; //50f
	[Range(0.1f, 4f)] [SerializeField] float steerSpeedInAir = 4f; //15
	[SerializeField] float gravity = 80f;
	[SerializeField] float maxFallSpeed = -2f;

	[Header("Jumping")]
	[SerializeField] JumpModule jumpModule;

	[Header("Slope")]
	[Tooltip("Maximum  slope angle")]
	[SerializeField] int maxSlope = 70; 

	[Header("Environment check")]
	[SerializeField] LayerMask groundLayer;

	//Reference
	Player2DRaycasts raycaster;
	Rigidbody2D rb;

	//Stats
	float coyoteAllowance = 0.2f;
	float jumpQueueAllowance = 0.05f;

	//Status
	Vector3 currentVelocity;
	bool isOnGround;
	bool isOnGroundPrevious;
	bool isJumping;
	int movingSign;
	bool debug_Decending;

	float coyoteTimer;
	float jumpQueueTimer;

	float moveXSmoothDampVelocity;

	float slopeAngle;

	//Cache
	float decendSlopeCheckDist;

	//Consts
	const float SkinWidth = 0.015f;


	#region Property
	public Vector3 GetVelocity => currentVelocity;
	float steerSpeed => isOnGround ? steerSpeedOnGround : steerSpeedInAir;
	bool hasWalkedOffPlatform => isOnGroundPrevious && !isOnGround && !isJumping;
	bool isFalling => currentVelocity.y < 0f;
	bool canJump => isOnGround || (coyoteTimer > 0f && !isJumping);
	bool isMovingUp => currentVelocity.y > 0f;
    #endregion

    #region Public
	public void SetVelocityY (float y)
    {
		currentVelocity.y = y;
    }
    #endregion

    #region MonoBehiavor
    void Awake()
	{
		//The maximum y distance for raycasting down, based on max slope level and max move Speed.
		decendSlopeCheckDist = moveSpeed * Mathf.Tan(maxSlope * Mathf.Deg2Rad);


		rb = GetComponent<Rigidbody2D>();
		raycaster = GetComponent<Player2DRaycasts>();
		jumpModule.Initialize(this);
	}

	void Update()
	{
		JumpDetection();
		TimerUpdate();
	}

	void FixedUpdate()
	{
        FacingSignCheck();
		PhysicsCheck();

		//Move
		HorizontalMoveUpdate();
		GravityUpdate();
		SlopeHandling();

		rb.velocity = currentVelocity;
		isOnGroundPrevious = isOnGround;
	}
	#endregion

	#region Pre-check
	void PhysicsCheck()
	{
		raycaster.UpdateOriginPoints();
		isOnGround = raycaster.IsOnGround;

		if (isMovingUp)
		{
			NudgeAwayFromCeilingEdge();
			CheckForCeilingHit();
		}

		LandingOvershootPrevention();
	}

	//Ceiling nudge
	void NudgeAwayFromCeilingEdge ()
    {
		float nudgeX = raycaster.CheckForCeilingSideNudge(currentVelocity.y * Time.deltaTime);
		if (nudgeX != 0f)
		{
			Vector3 p = rb.position;
			p.x += nudgeX;
			rb.position = p;
		}
	}

	void CheckForCeilingHit ()
    {
		if (raycaster.HitsCeiling)
		{
			jumpModule.OnBtnUp();
			currentVelocity.y = 0f;
			isJumping = false;
		}
	}

	//Stops the player from sliding on slopes on the frame that they lands.
	void LandingOvershootPrevention () 
    {
		//If falling while not moving sideways...
		if (movingSign == 0f && currentVelocity.y < 0f)
        {
			//And the falling velocity is going past the ground, then reduce the velocity.
			float distance = raycaster.DistanceToGround(-currentVelocity.y * Time.deltaTime);
			if (distance > 0)
            {
				currentVelocity.y = -distance / Time.deltaTime ;
				isOnGround = true;
			}
		}
    }
	#endregion

	#region Movement logic
	void GravityUpdate()
	{
		if (isOnGround)
		{
			CheckIfJustLanded();
		}
		else
		{
			ApplyGravity();

			//If just walked off a platform, reset coyote timer.
			if (hasWalkedOffPlatform)
			{
				coyoteTimer = coyoteAllowance;
			}
		}
	}
		
	void HorizontalMoveUpdate()
	{
		currentVelocity.x = Mathf.SmoothDamp(currentVelocity.x, GameInput.MoveX * moveSpeed, ref moveXSmoothDampVelocity, steerSpeed * Time.deltaTime);
	}
	#endregion

	#region Jumping logic
	void JumpDetection()
	{
		if (GameInput.JumpBtnDown)
        {
			if (canJump)
			{
				OnJumpBtnDown();
			}
		}

		if (GameInput.JumpBtn)
		{
			OnJumpBtnHold();
			jumpQueueTimer = jumpQueueAllowance;
		}

		if (GameInput.JumpBtnUp)
		{
			OnJumpBtnUp();
		}
	}

	void OnJumpBtnDown ()
    {
		isJumping = true;
		jumpQueueTimer = -1f;
		coyoteTimer = -1f;
		jumpModule.OnBtnDown();
	}

	void OnJumpBtnHold()
	{
		jumpModule.OnBtnHold();
	}

	void OnJumpBtnUp()
	{
		isJumping = false;
		jumpModule.OnBtnUp();

	}

	void ApplyGravity()
	{
		currentVelocity.y -= gravity * Time.deltaTime;

		//clamp gravity:
		if (currentVelocity.y < maxFallSpeed)
			currentVelocity = new Vector2(currentVelocity.x, maxFallSpeed);
	}

	void CheckIfJustLanded()
	{
		if (!isOnGroundPrevious && isOnGround)
		{
			isJumping = false;
			if (jumpQueueTimer > 0f) //Automatically jumps if player has queued a jump command.
			{
				OnJumpBtnDown();
			}
			else if (isFalling)
			{
				currentVelocity.y = 0;
			}
		}
	}
	#endregion

	#region Slop handling
	void SlopeHandling ()
    {
		if (currentVelocity.y <= 0f)
		{
			if (movingSign != 0) //Player is pressing a move command.
            {
				Vector2 origin = movingSign > 0 ? raycaster.BL : raycaster.BR; //The origin should be the backfoot
				StickToDecendingSlope(origin);
			}
			else if (isOnGround)
            {
				location = -1;
				//targetVelocity.x = 0f;
				currentVelocity.y = 0f;

				//targetVelocity = Vector3.zero;
			}
		}
	}

	int location = -1;
	void StickToDecendingSlope(Vector2 origin)
	{
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, 10f, groundLayer); //20f is an arbitrary large number.
		
		debug_Decending = false;
		if (hit)
		{
			slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
			//If the slope is less than maxSlope angle
			if (slopeAngle != 0 && slopeAngle < maxSlope)
			{
				//See if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
				if (Mathf.Sign(hit.normal.x) == movingSign)
				{
					debug_Decending = true;
					//Check if we are standing close enough to the platform to begin decend calculation. 
					float descendableRange = decendSlopeCheckDist * Time.deltaTime;
					if (hit.distance - SkinWidth < descendableRange)
					{
						//Specify the decend amount
						//Btw we're using max move speed (moveSpeed) instead of currentVelocity.x because it is reduced by smoothdamp.
						currentVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveSpeed * movingSign;
						currentVelocity.y = -Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveSpeed;
						//Make the player move towards the slop if it is hovering above it
						currentVelocity.y -= (hit.distance - SkinWidth - SkinWidth)  / Time.deltaTime ; 
                    }
				}
			}
		}
	}
	#endregion

	#region Util
	void TimerUpdate()
	{
		if (coyoteTimer > 0f)
		{
			coyoteTimer -= Time.deltaTime;
		}

		if (jumpQueueTimer > 0f)
		{
			jumpQueueTimer -= Time.deltaTime;
		}
	}

	void FacingSignCheck ()
    {
		if (GameInput.MoveX > 0.1f)
		{
			movingSign = 1;
		}
		else if (GameInput.MoveX < -0.1f)
		{
			movingSign = -1;
		}
		else
        {
			movingSign = 0;
        }
	}
	#endregion

	
	private void OnGUI()
	{
		GUI.Label(new Rect(20, 0,		290, 20), "=== GROUND MOVE === ");
		GUI.Label(new Rect(20, 20,		290, 20), "OnGround: " + isOnGround);
		GUI.Label(new Rect(20, 40,		290, 20), "onGroundPrevious: " + isOnGroundPrevious);
		GUI.Label(new Rect(20, 60,		290, 20), "GameInput.MoveX: " + GameInput.MoveX);

		GUI.Label(new Rect(20, 120, 290, 20), "targetVelocity: " + currentVelocity);

		GUI.Label(new Rect(300, 0,		290, 20), "=== JUMPING === ");
		GUI.Label(new Rect(300, 20,		290, 20), "coyoteTimer: " + coyoteTimer);
		GUI.Label(new Rect(300, 40,		290, 20), "jumpQueueTimer: " + jumpQueueTimer);
		GUI.Label(new Rect(300, 60,		290, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);
		GUI.Label(new Rect(300, 80,		290, 20), "jumping: " + isJumping);

		GUI.Label(new Rect(500, 0,		290, 20), "=== SLOPE === ");
		GUI.Label(new Rect(500, 20,	290, 20), "decending: " + debug_Decending);
		GUI.Label(new Rect(500, 40,	290, 20), "slopeAngle: " + slopeAngle);
		GUI.Label(new Rect(500, 60, 290, 20), "location: " + location);
	}
}
