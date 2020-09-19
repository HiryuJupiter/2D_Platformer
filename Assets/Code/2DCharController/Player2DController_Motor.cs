using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//Coyote time: A short time period allows the player to jump after walking off a platform.
//Jump queue timer: Allows the player to "cache" the jump command when pressing jump right before land on ground.


[RequireComponent(typeof(Player2DRaycasts))]
[RequireComponent(typeof(Player2DController_Graphics))]
public class Player2DController_Motor : MonoBehaviour
{
	[Header("Move Speed")]
	[SerializeField] float moveSpeed = 15f;
	//[Range(0, 1)]	[SerializeField] float crouchMoveSpeed = .36f;
	[Range(5f, 50f)] [SerializeField] float steeringOnGround = 50f;
	[Range(5f, 50f)] [SerializeField] float steeringInAir = 15f;

	[Header("Jumping")]
	[SerializeField] float jumpForce = 25f;
	
	[SerializeField] LayerMask groundLayer;

	[Header("Gravity")]
	[SerializeField] float gravity = 100f;

	[Header("Slope")]
	[Range(1f, 89f)] [SerializeField] float maxSlope = 80f;

	float coyoteAllowance = 0.2f;
	float jumpQueueAllowance = 0.2f;

	//[Header("References")]
	//[SerializeField] Collider2D collider_standing;

	//Components and classes
	Player2DController_Graphics graphics;
	Player2DRaycasts raycast;
	Rigidbody2D rb;

	//Status
	Vector3 targetVelocity;
	bool onGround;
	bool onGroundPrevious;
	bool jumping;
	float coyoteTimer; 
	float jumpQueueTimer;
	float facingSign;
	//bool crouching;

	//float crouchMoveModifier;
	//Cache
	float maxSlopeTangent; //To make slope calculations faster

	//Ignore
	float smoothDampVelocity;

	//Consts
	const float SkinWidth = 0.015f;

	#region Property
	float steering => onGround ? steeringOnGround : steeringInAir;
	bool DetectsGroundWhileFalling => !onGroundPrevious && onGround && Falling;
	bool WalkedOffPlatform => onGroundPrevious && !onGround && !jumping;
	bool Falling => rb.velocity.y < 0f;
	bool MovingRight => GameInput.MoveX > 0.1f;
	bool MovingLeft => GameInput.MoveX < -0.1f;
	bool CanJump => onGround || (coyoteTimer > 0f && !jumping);
	#endregion

	#region MonoBehavior
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		raycast = GetComponent<Player2DRaycasts>();
		graphics = GetComponent<Player2DController_Graphics>();
		//crouching = false;

		maxSlopeTangent = Mathf.Tan(maxSlope * Mathf.Deg2Rad);
	}

    void Update()
    {
		JumpDetection();
		TimerUpdate();
	}
    void FixedUpdate()
	{
		CacheCalculationValues();

		//Move
		HorizontalMoveUpdate();
		VerticalMoveUpdate();

		rb.velocity = targetVelocity;
	}
	#endregion

	#region Movement updates
	void VerticalMoveUpdate ()
    {
		if (onGround)
        {
			targetVelocity.y = 0f;
			if (GameInput.MoveX != 0)
            {
				StickToDecendingSlope();
			}
			else
            {

            }
		}
		else //If not on ground
        {
			ApplyGravity();

			//If just walked off a platform, reset coyote timer.
			if (WalkedOffPlatform)
			{
				coyoteTimer = coyoteAllowance;
			}

			if (DetectsGroundWhileFalling)
			{
				Lands();
			}
		}
	}


	void HorizontalMoveUpdate ()
    {
		targetVelocity.x = Mathf.Lerp(targetVelocity.x, GameInput.MoveX * moveSpeed, steering * Time.deltaTime);
	}

	void JumpDetection()
    {
        if (GameInput.JumpBtn)
        {
			if (CanJump)
            {
				ApplyJump();
			}
			else
            {
				jumpQueueTimer = jumpQueueAllowance;
			}
        }
    }
	#endregion

	#region Collision
	void StickToDecendingSlope()
	{
		Vector2 origin = facingSign > 0? raycast.BR : raycast.BL;
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, 100f, groundLayer);
		if (hit)
		{
			float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);

			//If the slope is less than maxSlope angle
			if (slopeAngle != 0 && slopeAngle < maxSlope)
			{
				//Check if we're decending the slope, by checking if we are facing the same x-direction as the slope normal
				if (Mathf.Sign(hit.normal.x) == facingSign)
				{
					//Check if we are standing close enough to the platform to begin decend calculation. 
					float descendableRange = (Mathf.Abs(GameInput.MoveX) * Mathf.Tan(slopeAngle * Mathf.Deg2Rad));
					if (hit.distance - SkinWidth < descendableRange)
					{
						//Specify the decend amount
						float moveDist = Mathf.Abs(GameInput.MoveX);
						testtemp = moveDist;
						Debug.DrawRay(hit.point, hit.normal, Color.red);
						targetVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * facingSign;
						targetVelocity.y = -Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
					}
					else
					{
						Debug.DrawRay(hit.point, hit.normal, Color.yellow);
					}
				}
			}
			else
            {
				targetVelocity.y = 0f;

			}
		}
	}
	#endregion
	float testtemp;
	#region Movement events
	void ApplyJump()
	{
		jumping = true;
		targetVelocity.y = jumpForce;

		jumpQueueTimer = -1f;
		coyoteTimer = -1f;
	}

	void ApplyGravity()
	{
		targetVelocity.y -= gravity * Time.deltaTime;
	}

	void Lands()
	{
		if (jumpQueueTimer <= 0f)
		{
			onGround = true;
			jumping = false;
			targetVelocity.y = 0;
		}
		else
		{
			ApplyJump();
		}
	}

	#endregion

	#region Util
	void CacheCalculationValues ()
    {

		onGroundPrevious = onGround;
		onGround = raycast.OnGround;
		UpdateFacing();
		raycast.UpdateOriginPoints();
	}

	void UpdateFacing ()
    {
		if (GameInput.MoveX > 0.1f)
		{
			facingSign = 1f;
		}
		else if (GameInput.MoveX < -0.1f)
		{
			facingSign = -1f;
		}

		if (MovingRight && !graphics.facingRight)
		{
			graphics.SetFacing(true);
		}
		else if (MovingLeft && graphics.facingRight)
		{
			graphics.SetFacing(false);
		}
	}

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
	#endregion

	private void OnGUI()
    {
        GUI.Label(new Rect(500, 20, 500, 20), "OnGround: " + onGround);
		GUI.Label(new Rect(500, 40, 500, 20), "onGroundPrevious: " + onGroundPrevious);
		GUI.Label(new Rect(500, 60, 500, 20), "coyoteTimer: " + coyoteTimer);
		GUI.Label(new Rect(500, 80, 500, 20), "jumpQueueTimer: " + jumpQueueTimer);
		GUI.Label(new Rect(500, 100, 500, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);

		GUI.Label(new Rect(500, 120, 500, 20), "jumping: " + jumping);

		GUI.Label(new Rect(500, 140, 500, 20), "OnGround: " + onGround);

		GUI.Label(new Rect(500, 160, 500, 20), "facingSign: " + facingSign);
		GUI.Label(new Rect(500, 180, 500, 20), "GameInput.MoveX: " + GameInput.MoveX);

		GUI.Label(new Rect(500, 200, 500, 20), "targetVelocity: " + targetVelocity);
		




	}
}