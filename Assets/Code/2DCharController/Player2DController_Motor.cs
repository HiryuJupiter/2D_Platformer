using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*Coyote time: A short time period allows the player to jump after walking off a platform.
Jump queue timer: Allows the player to "cache" the jump command when pressing jump right before land on ground.
Jumping and OnGround are two different situations that doesn't always overlap. After pressing jump, you're still onground for a few frames
due to the raycast distance. And you can also walk off a platform that cause you to become not-on-ground and no-jumping.

 */



[RequireComponent(typeof(Player2DRaycasts))]
[RequireComponent(typeof(Player2DController_Graphics))]
public class Player2DController_Motor : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] float moveSpeed = 15f;
	//[Range(0, 1)]	[SerializeField] float crouchMoveSpeed = .36f;
	[Range(5f, 50f)] [SerializeField] float steeringOnGround = 50f;
	[Range(5f, 50f)] [SerializeField] float steeringInAir = 15f;
	[SerializeField] float gravity = 100f;
	[SerializeField] float maxFallSpeed = -10f;

	[Header("Jumping")]
	[SerializeField] float minJumpForce = 25f;
	[SerializeField] float maxJumpForce = 35f;

	[Header("Environment check")]
	[SerializeField] LayerMask groundLayer;
	


	float coyoteAllowance = 0.2f;
	float jumpQueueAllowance = 0.2f;

	//[Header("References")]
	//[SerializeField] Collider2D collider_standing;

	//Components and classes
	Player2DController_Graphics graphics;
	Player2DRaycasts raycaster;
	Rigidbody2D rb;

	//Status
	Vector3 targetVelocity;
	bool isOnGround;
	bool isOnGroundPrevious;
	bool isJumping;

	float coyoteTimer; 
	float jumpQueueTimer;

	int facingSign;

	//Ignore
	float smoothDampVelocity;

	//Consts
	const float SkinWidth = 0.015f;

	#region Property
	float steering => isOnGround ? steeringOnGround : steeringInAir;
	bool WalkedOffPlatform => isOnGroundPrevious && !isOnGround && !isJumping;
	bool Falling => rb.velocity.y < 0f;
	bool CanJump => isOnGround || (coyoteTimer > 0f && !isJumping);
	#endregion

	#region MonoBehavior
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		raycaster = GetComponent<Player2DRaycasts>();
		graphics = GetComponent<Player2DController_Graphics>();
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
	}
	#endregion


	void PhysicsCheck()
	{
		raycaster.UpdateOriginPoints();
		isOnGround = raycaster.IsOnGround;
		UpdateFacing();
	}

	#region Movement updates
	void VerticalMoveUpdate ()
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


	void HorizontalMoveUpdate ()
    {
		targetVelocity.x = Mathf.Lerp(targetVelocity.x, GameInput.MoveX * moveSpeed, steering * Time.deltaTime);
	}

	void JumpDetection()
    {
        if (GameInput.JumpBtnDown)
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
	#endregion

	#region Movement logic
	void OnJumpInputDown()
	{
		isJumping = true;
		targetVelocity.y = maxJumpForce;
		jumpQueueTimer = -1f;
		coyoteTimer = -1f;
	}

	void OnJumpInputUp ()
    {
		isJumping = false;
		if (targetVelocity.y > minJumpForce)
        {
			targetVelocity.y = minJumpForce;
		}
    }

	void ApplyGravity()
	{
		targetVelocity.y -= gravity * Time.deltaTime;

		//clamp gravity:
		if (rb.velocity.y < maxFallSpeed)
			rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
	}

	void CheckIfJustLanded()
	{
		if(!isOnGroundPrevious && isOnGround)
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


	void UpdateFacing ()
    {
		if (GameInput.MoveX > 0.1f)
		{
			facingSign = 1;
		}
		else if (GameInput.MoveX < -0.1f)
		{
			facingSign = -1;
		}

		graphics.SetFacing(GameInput.MoveX);
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
        GUI.Label(new Rect(500, 20, 500, 20), "OnGround: " + isOnGround);
		GUI.Label(new Rect(500, 40, 500, 20), "onGroundPrevious: " + isOnGroundPrevious);
		GUI.Label(new Rect(500, 60, 500, 20), "coyoteTimer: " + coyoteTimer);
		GUI.Label(new Rect(500, 80, 500, 20), "jumpQueueTimer: " + jumpQueueTimer);
		GUI.Label(new Rect(500, 100, 500, 20), "GameInput.JumpBtnDown: " + GameInput.JumpBtnDown);

		GUI.Label(new Rect(500, 120, 500, 20), "jumping: " + isJumping);

		GUI.Label(new Rect(500, 140, 500, 20), "OnGround: " + isOnGround);

		GUI.Label(new Rect(500, 160, 500, 20), "facingSign: " + facingSign);
		GUI.Label(new Rect(500, 180, 500, 20), "GameInput.MoveX: " + GameInput.MoveX);

		GUI.Label(new Rect(500, 200, 500, 20), "targetVelocity: " + targetVelocity);
		




	}
}