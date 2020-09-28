using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

/*Coyote time: A short time period allows the player to jump after walking off a platform.
Jump queue timer: Allows the player to "cache" the jump command when pressing jump right before land on ground.
Jumping and OnGround are two different situations that doesn't always overlap. After pressing jump, you're still onground for a few frames
due to the raycast distance. And you can also walk off a platform that cause you to become not-on-ground and no-jumping.
 */

/*
//For all slope movements, we use the maximum speed, moveSpeed, instead of currentVelocity that gets reduced by
//smoothDamp, as you're lerping from the slope climbing/descending x-positional displacement towards the maxspeed,
//which will make the slower than what it should be
 */

/*
[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(Player2DRaycaster))]
[RequireComponent(typeof(JumpModule))]
public class Player2DController_Motor : MonoBehaviour
{
	//[Header("References")]
	//[SerializeField] Collider2D collider_standing;
	public bool stickyGround = true;

	[Header("Movement")]
	[SerializeField] float moveSpeed = 60f;
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
	Player2DRaycaster raycaster;
	Rigidbody2D rb;

	//Stats


	//Status
	Vector3 currentVelocity;
	bool isOnGround;
	bool isOnGroundPrevious;
	bool isJumping;
	int movingSign;

	float coyoteTimer;
	float jumpQueueTimer;

	bool descendingSlope;
	bool climbingSlope;
	float slopeAngleOld;
	float slopeAngle;

	//Cache
	float decendSlopeCheckDist;

	//Consts
	const float MaxCoyoteDuration = 0.2f;
	const float MaxJumpQueueDuration = 0.05f;
	const float SkinWidth = 0.005f;


	#region Properties & expression body methods
	public Vector3 GetVelocity() => currentVelocity;
	float steerSpeed => isOnGround ? steerSpeedOnGround : steerSpeedInAir;
	bool isFalling => currentVelocity.y < 0f;
	bool canJump => isOnGround || (coyoteTimer > 0f && !isJumping);
	bool isMovingUp => currentVelocity.y > 0f;
	bool isMoving => movingSign != 0;
	#endregion

	#region Public
	public void SetVelocityY(float y)
	{
		currentVelocity.y = y;
	}

	public void SetVelocity(Vector2 velocity)
	{
		currentVelocity = velocity;
	}
	#endregion

	#region MonoBehiavor
	void Awake()
	{
		//Caching the maximum raycast distance for slope calculations, based on max slope level and max move Speed.
		decendSlopeCheckDist = moveSpeed * Mathf.Tan(maxSlope * Mathf.Deg2Rad);

		rb = GetComponent<Rigidbody2D>();
		raycaster = GetComponent<Player2DRaycaster>();
		jumpModule.Initialize(this);
	}

	void Update()
	{

		JumpDetection();
		UpdateTimer();
	}

	void FixedUpdate()
	{
		raycaster.UpdateOriginPoints();
		UpdateFacingSign();
		PhysicsCheck();

		//Horizontal move
		CheckIfWalkedOffPlatform();
		UpdateHorizontalMove();

		//Vertical move
		ApplyGravity();
		GravityOvershootPrevention();
		CheckIfJustLanded();

		//Slope
		if (stickyGround && !isJumping)
		{
			StickToGround();
		}

		rb.velocity = currentVelocity;
		isOnGroundPrevious = isOnGround;
		slopeAngleOld = slopeAngle;
	}
	#endregion

	#region Jumping logic
	void JumpDetection()
	{
		if (GameInput.JumpBtnDown && canJump)
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
		isJumping = true;
		jumpQueueTimer = -1f;
		coyoteTimer = -1f;
		jumpModule.OnBtnDown();
	}

	void OnJumpBtnHold()
	{
		jumpQueueTimer = MaxJumpQueueDuration;
		jumpModule.OnBtnHold();
	}

	void OnJumpBtnUp()
	{
		//isJumping = false;
		jumpModule.OnBtnUp();
	}
	#endregion

	#region Pre-check
	void PhysicsCheck()
	{
		isOnGround = raycaster.IsOnGround;

		if (isMovingUp)
		{
			NudgeAwayFromCeilingEdge();
			CheckForCeilingHit();
		}
	}

	void NudgeAwayFromCeilingEdge()
	{
		float nudgeX = raycaster.CheckForCeilingSideNudge(currentVelocity.y * Time.deltaTime);
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
			jumpModule.OnBtnUp();
			currentVelocity.y = 0f;
			isJumping = false;
		}
	}
	#endregion

	#region Horizontal move
	void CheckIfWalkedOffPlatform()
	{
		if (!isOnGround && isOnGroundPrevious && !isJumping)
		{
			StartCoyoteTimer();
		}
	}

	float moveXSmoothDampVelocity;
	void UpdateHorizontalMove()
	{
		currentVelocity.x = Mathf.SmoothDamp(currentVelocity.x, GameInput.MoveX * moveSpeed, ref moveXSmoothDampVelocity, steerSpeed * Time.deltaTime);
	}
	#endregion

	#region Gravity & Landing
	void ApplyGravity()
	{
		if (!isOnGround)
		{
			currentVelocity.y -= gravity * Time.deltaTime;
			currentVelocity.y = Mathf.Clamp(currentVelocity.y, maxFallSpeed, currentVelocity.y);
		}
	}

	//Stops the player from sliding on slopes on the frame that they lands.
	void GravityOvershootPrevention()
	{
		if (isFalling && !isMoving)
		{
			//If the falling velocity is going below the ground, then reduce the velocity.
			float distance = raycaster.DistanceToGround(-currentVelocity.y * Time.deltaTime);
			if (distance > 0)
			{
				Debug.DrawRay(raycaster.BR, Vector3.right, Color.yellow);
				Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.green);

				RaycastHit2D right = Physics2D.Raycast(raycaster.BR, Vector2.down, -currentVelocity.y * Time.deltaTime, groundLayer);
				Debug.DrawRay(right.point, Vector3.right, Color.magenta);

				currentVelocity.y = -distance; //THis is absolutely perfct in Non-interpolate. Interpolation does make the character slide upon landing, but this is by far the best option.
											   //Debug.DrawRay((Vector3)raycaster.BR - Vector3.down * currentVelocity.y * Time.deltaTime, Vector3.right, Color.cyan);
			}
		}
	}

	void CheckIfJustLanded()
	{
		if (!isOnGroundPrevious && isOnGround)
		{
			isJumping = false;
			coyoteTimer = -1f;

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

	#region Slope handling
	void StickToGround()
	{
		if (isMoving)
		{
			climbingSlope = false;
			descendingSlope = false;

			//Prioritize checking if climbing slope 
			Vector2 frontfoot = movingSign > 0 ? raycaster.BR : raycaster.BL;
			AscendingSlope(frontfoot);

			if (!climbingSlope)
			{
				//Check if can descend slope if not currently climbing one
				Vector2 backfoot = movingSign > 0 ? raycaster.BL : raycaster.BR;
				StickToDecendingSlope(backfoot);

				//This prevents "car-flies-over-ramp" effect when finishing climbing.
				if (!descendingSlope && !isOnGround)
				{
					//Stick to ground
					if (currentVelocity.y > 0f)
					{
						currentVelocity.y = 0f;
					}
				}
			}
		}
		else
		{
			if (isOnGround)
			{
				//Don't let player slide down a slope by gravity.
				currentVelocity.y = 0f;
			}
		}
	}

	void AscendingSlope(Vector2 origin)
	{
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.right * movingSign, Mathf.Abs(currentVelocity.x) * Time.deltaTime, groundLayer);

		Debug.DrawRay(origin, new Vector3(currentVelocity.x * Time.deltaTime, 0f, 0f), Color.cyan);

		if (hit)
		{
			Debug.DrawRay(hit.point, hit.normal, Color.green);

			float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
			if (slopeAngle != 0 && slopeAngle < maxSlope)
			{
				climbingSlope = true;
				Vector3 newVelocity = Vector3.zero;
				float gapDist = 0f;
				//If there is space between you and the slope, then move right up against it.
				if (slopeAngle != slopeAngleOld) //For optimization, only do once per slope
				{
					gapDist = hit.distance - SkinWidth;
					newVelocity.x = gapDist / Time.deltaTime * movingSign;
				}
				//Take the full VelocityX, minus the gap distance, then use the remaining velocity X...
				//...to calculate slope climbing. 
				float climbDistance = moveSpeed - gapDist; //climbDistance is also the hypotenues
				float displaceX = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * climbDistance * movingSign;
				float displaceY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * climbDistance;
				newVelocity.x += displaceX;
				newVelocity.y = displaceY;

				currentVelocity = newVelocity;
			}
		}
	}

	void StickToDecendingSlope(Vector2 origin)
	{
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, decendSlopeCheckDist * Time.deltaTime, groundLayer);
		//Debug.DrawRay(origin, Vector3.down * decendSlopeCheckDist * Time.deltaTime, Color.red, 0.5f);

		if (hit)
		{
			//Debug.DrawRay(hit.point, hit.normal, Color.magenta, 0.5f);
			slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
			//If the slope is less than maxSlope angle
			if (slopeAngle != 0 && slopeAngle < maxSlope)
			{
				//See if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
				if (Mathf.Sign(hit.normal.x) == movingSign)
				{
					descendingSlope = true;
					//Check if we are standing close enough to the platform to begin decend calculation. 
					//float descendableRange = decendSlopeCheckDist ;
					//if (hit.distance - SkinWidth < descendableRange)
					{
						//Specify the decend amount
						//Btw we're using max move speed (moveSpeed) instead of currentVelocity.x because it is reduced by smoothdamp.
						currentVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveSpeed * movingSign;
						currentVelocity.y = -Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveSpeed;
						//currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
						if (slopeAngle != slopeAngleOld)
						{
							//Make the player move towards the slop if it is hovering above it
							//We use slopAngleOld for performance optimization
							currentVelocity.y -= (hit.distance - SkinWidth) / Time.deltaTime;
						}
					}
				}
			}
		}
	}
	#endregion

	#region Util
	void UpdateTimer()
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

	void UpdateFacingSign()
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
	void StartCoyoteTimer() => coyoteTimer = MaxCoyoteDuration;
	#endregion

	void OnGUI()
	{
		GUI.Label(new Rect(20, 0, 290, 20), "=== GROUND MOVE === ");
		GUI.Label(new Rect(20, 20, 290, 20), "OnGround: " + isOnGround);
		GUI.Label(new Rect(20, 40, 290, 20), "onGroundPrevious: " + isOnGroundPrevious);
		GUI.Label(new Rect(20, 60, 290, 20), "GameInput.MoveX: " + GameInput.MoveX);
		GUI.Label(new Rect(20, 80, 290, 20), "movingSign: " + movingSign);


		GUI.Label(new Rect(20, 120, 290, 20), "targetVelocity: " + currentVelocity);

		GUI.Label(new Rect(300, 0, 290, 20), "=== JUMPING === ");
		GUI.Label(new Rect(300, 20, 290, 20), "coyoteTimer: " + coyoteTimer);
		GUI.Label(new Rect(300, 40, 290, 20), "jumpQueueTimer: " + jumpQueueTimer);
		GUI.Label(new Rect(300, 60, 290, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);
		GUI.Label(new Rect(300, 80, 290, 20), "jumping: " + isJumping);

		//GUI.Label(new Rect(300, 120,		290, 20), "testLocation: " + testLocation);



		GUI.Label(new Rect(500, 0, 290, 20), "=== SLOPE === ");
		GUI.Label(new Rect(500, 20, 290, 20), "decending: " + descendingSlope);
		GUI.Label(new Rect(500, 40, 290, 20), "climbingSlope: " + climbingSlope);
		GUI.Label(new Rect(500, 60, 290, 20), "slopeAngle: " + slopeAngle);
	}
}
*/