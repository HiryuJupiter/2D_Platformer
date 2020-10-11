using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-9000)]
public class GameSettings : MonoBehaviour
{
    public static GameSettings instance { get; private set; }

    [Header("Layers")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask playerLayer;
    public LayerMask GroundLayer => groundLayer;
    public LayerMask PlayerLayer => playerLayer;


    [Header("Player Movement")]
    [Range(0.1f, 4f)] [SerializeField] float steerSpeedGround = 1f; //50f
    [Range(0.1f, 6f)] [SerializeField] float steerSpeedAir = 5f; //50f
    [SerializeField] float playerMoveSpeed = 10;
    [SerializeField] float playerCrawlSpeed = 6;
    public float SteerSpeedGround => steerSpeedGround;
    public float SteerSpeedAir => steerSpeedAir;
    public float PlayerMoveSpeed => playerMoveSpeed;
    public float PlayerCrawlSpeed => playerCrawlSpeed;


    [Header("Normal Jump")]
    [SerializeField] float minJumpForce = 12f;
    [SerializeField] float maxJumpForce = 22f;
    [SerializeField] float maxCoyoteDuration = 0.25f;
    public float MinJumpForce => minJumpForce;
    public float MaxJumpForce => maxJumpForce;
    public float MaxCoyoteDuration => maxCoyoteDuration;

    [Header("Wall Jump")]
    [SerializeField] Vector2 wallJumpUpForce = new Vector2(20f, 25f); //7.5, 16
    [SerializeField] Vector2 wallJumpAwayForce = new Vector2(20f, 22f); //7.5, 16

    [SerializeField] Vector2 wallDetachForce = new Vector2(1f, 7f); //8.5, 7
    
    [SerializeField] float wallStickMaxDuration = .25f;
    [SerializeField] float wallSlideSpeedSlow = 2.5f;
    [SerializeField] float wallSlideSpeedFast = 4f;
    public Vector2 WallJumpUpForce => wallJumpUpForce;
    public Vector2 WallJumpAwayForce => wallJumpAwayForce;
    public Vector2 WallDetachForce => wallDetachForce;
    public float WallStickMaxDuration => wallStickMaxDuration;
    public float WallSlideSpeedSlow => wallSlideSpeedSlow;
    public float WallSlideSpeedFast => wallSlideSpeedFast;

    [Header("Hurt State")]
    [Range(10f, 50f)] [SerializeField] float hurtSlideSpeed = 20f; //50f

    [SerializeField] Vector2 hurtDirection = new Vector2(20f, 25f);
    [SerializeField] float hurtDuration = 0.5f;
    public float HurtSlideSpeed => hurtSlideSpeed;
    public Vector2 HurtDirection => hurtDirection;
    public float HurtDuration => hurtDuration;

    [Header("Gravity")]
    
    [SerializeField] float maxFallSpeed = -15f;
    [SerializeField] float gravity = 80f;
    public float MaxFallSpeed => maxFallSpeed;
    public float Gravity => gravity;

    [Header("Crouch")]
    [SerializeField] Vector2 crouchOffset;
    [SerializeField] Vector2 crouchSize;
    public Vector2 CrouchOffset => crouchOffset;
    public Vector2 CrouchSize => crouchSize;

    [Header("Physics  behavior")]
    [SerializeField] bool stickyGround = true;
    [SerializeField] int maxSlopeAngle = 70;
    //[Range(0.05f, 0.2f)] [SerializeField] float angleBasedFallSpeedModifier = 0.02f;
    public bool StickyGround => stickyGround;
    public int MaxSlopeAngle => maxSlopeAngle;
    //public float AngleBasedFallSpeedModifier => angleBasedFallSpeedModifier;

    void Awake()
    {
        //Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}