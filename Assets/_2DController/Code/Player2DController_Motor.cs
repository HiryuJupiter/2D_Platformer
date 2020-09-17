using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Player2DRaycasts))]
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

	float coyoteAllowance = 0.2f;
	float jumpQueueAllowance = 0.2f;

	//[Header("References")]
	//[SerializeField] Collider2D collider_standing;

	//Components and classes
	Player2DRaycasts raycast;
	Rigidbody2D rb;

	//Status
	Vector3 targetVelocity;
	bool onGround;
	bool onGroundPrevious;
	bool jumping;
	float coyoteTimer;
	float jumpQueueTimer;
	//bool crouching;
	bool facingRight = true;
	//float crouchMoveModifier;

	//Ignore
	float smoothDampVelocity;

	#region Property
	float steering => onGround ? steeringOnGround : steeringInAir;
	bool DetectsGroundInMidair => !onGroundPrevious && onGround;
	bool WalkedOffPlatform => onGroundPrevious && !onGround && !jumping;
	bool Falling => rb.velocity.y <= 0f;
	bool MovingRight => GameInput.MoveX > 0.1f;
	bool MovingLeft => GameInput.MoveX < -0.1f;
	bool CanJump => onGround || (coyoteTimer > 0f && !jumping);
	#endregion

	#region MonoBehavior
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		raycast = GetComponent<Player2DRaycasts>();
		//crouching = false;
	}

    void Update()
    {
		JumpDetection();
		TimerUpdate();
	}
    void FixedUpdate()
	{
		//Cache values
		onGroundPrevious = onGround;
		onGround = raycast.OnGround;

		//Move
		ApplyMoveSpeed();
		FacingUpdate();

		AerialUpdate();

		rb.velocity = targetVelocity;
	}
	#endregion

	#region Private methods
	void AerialUpdate ()
    {
		if (!onGround)
        {
			ApplyGravity();
		}

		if (WalkedOffPlatform)
        {
			coyoteTimer = coyoteAllowance;
		}

		if (DetectsGroundInMidair && Falling)
		{
			Lands();
		}
	}

	void FacingUpdate()
	{
		if (MovingRight && !facingRight)
        {
			SetFacing(true);
		}
		else if (MovingLeft && facingRight)
		{
			SetFacing(false);
		}
	}

	void ApplyMoveSpeed ()
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

    void ApplyJump()
    {
		jumping = true;
		targetVelocity.y = jumpForce;

		jumpQueueTimer = -1f;
		coyoteTimer = -1f;
	}

	void ApplyGravity ()
    {
		targetVelocity.y -= gravity * Time.deltaTime;
	}

	void Lands ()
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

	void SetFacing(bool faceRight)
	{
		facingRight = faceRight;
		Vector3 scale = transform.localScale;
		scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
		transform.localScale = scale;
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

	}
}