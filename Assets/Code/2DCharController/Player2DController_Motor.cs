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
	[Range(0.1f, 4f)] [SerializeField] float steeringOnGround = 1f; //50f
	[Range(0.1f, 4f)] [SerializeField] float steeringInAir = 4f; //15
	[SerializeField] float gravity = 80f;
	[SerializeField] float maxFallSpeed = -2f;

	[Header("Jumping")]
	[SerializeField] JumpModule jumpModule;

	[Header("Environment check")]
	[SerializeField] LayerMask groundLayer;

	//Reference
	Player2DRaycasts raycaster;
	Rigidbody2D rb;

	//Stats
	float coyoteAllowance = 0.2f;
	float jumpQueueAllowance = 0.05f;

	//Status
	Vector3 targetVelocity;
	bool isOnGround;
	bool isOnGroundPrevious;
	bool isJumping;

	float coyoteTimer;
	float jumpQueueTimer;

	float smoothDampVelocity;

	//Consts
	const float SkinWidth = 0.015f;

	#region Property
	public Vector3 GetVelocity => targetVelocity;
	float steering => isOnGround ? steeringOnGround : steeringInAir;
	bool WalkedOffPlatform => isOnGroundPrevious && !isOnGround && !isJumping;
	bool Falling => targetVelocity.y < 0f;
	bool CanJump => isOnGround || (coyoteTimer > 0f && !isJumping);
    #endregion

    #region Public
	public void SetVelocityY (float y)
    {
		targetVelocity.y = y;
    }
    #endregion

    #region MonoBehiavor
    void Awake()
	{
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
		PhysicsCheck();

		//Move
		HorizontalMoveUpdate();
		VerticalMoveUpdate();

		rb.velocity = targetVelocity;
		isOnGroundPrevious = isOnGround;
	}
	#endregion

	#region Pre-check
	void PhysicsCheck()
	{
		raycaster.UpdateOriginPoints();
		isOnGround = raycaster.IsOnGround;
		if (raycaster.IsAgainstCeiling && isJumping && targetVelocity.y > 0f)
		{
			targetVelocity.y = 0f;
			isJumping = false;
		}
	}
	#endregion


	#region Movement logic
	void VerticalMoveUpdate()
	{
		if (isOnGround)
		{
			CheckIfJustLanded();
		}
		else
		{
			ApplyGravity();

			//If just walked off a platform, reset coyote timer.
			if (WalkedOffPlatform)
			{
				coyoteTimer = coyoteAllowance;
			}
		}
	}


	float moveXSmoothDampVelocity;
	void HorizontalMoveUpdate()
	{
		targetVelocity.x = Mathf.SmoothDamp(targetVelocity.x, GameInput.MoveX * moveSpeed, ref moveXSmoothDampVelocity, steering * Time.deltaTime);

		//targetVelocity.x = Mathf.Lerp(targetVelocity.x, GameInput.MoveX * moveSpeed, steering * Time.deltaTime);
	}
	#endregion

	#region Jumping logic
	void JumpDetection()
	{
		if (GameInput.JumpBtn)
		{
			if (CanJump)
			{
				OnJumpInputDown();
			}
			else
			{
				jumpQueueTimer = jumpQueueAllowance;
			}
		}

		if (GameInput.JumpBtnUp)
		{
			OnJumpInputUp();
		}
	}

	void OnJumpInputDown()
	{
		isJumping = true;
		jumpModule.StartApplyingJumpForce();
		jumpQueueTimer = -1f;
		coyoteTimer = -1f;
	}

	void OnJumpInputUp()
	{
		isJumping = false;
		jumpModule.StopApplyingJumpForce();

	}

	void ApplyGravity()
	{
		targetVelocity.y -= gravity * Time.deltaTime;

		//clamp gravity:
		if (targetVelocity.y < maxFallSpeed)
			targetVelocity = new Vector2(targetVelocity.x, maxFallSpeed);
	}

	void CheckIfJustLanded()
	{
		if (!isOnGroundPrevious && isOnGround)
		{
			isJumping = false;
			if (jumpQueueTimer > 0f) //Automatically jumps if player has queued a jump command.
			{
				OnJumpInputDown();
			}
			else if (Falling)
			{
				targetVelocity.y = 0;
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
	#endregion

	private void OnGUI()
	{
		GUI.Label(new Rect(500, 20, 500, 20), "OnGround: " + isOnGround);
		GUI.Label(new Rect(500, 40, 500, 20), "onGroundPrevious: " + isOnGroundPrevious);
		GUI.Label(new Rect(500, 60, 500, 20), "coyoteTimer: " + coyoteTimer);
		GUI.Label(new Rect(500, 80, 500, 20), "jumpQueueTimer: " + jumpQueueTimer);
		GUI.Label(new Rect(500, 100, 500, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);

		GUI.Label(new Rect(500, 120, 500, 20), "jumping: " + isJumping);

		GUI.Label(new Rect(500, 140, 500, 20), "OnGround: " + isOnGround);

		GUI.Label(new Rect(500, 180, 500, 20), "GameInput.MoveX: " + GameInput.MoveX);

		GUI.Label(new Rect(500, 200, 500, 20), "targetVelocity: " + targetVelocity);
	}
}
