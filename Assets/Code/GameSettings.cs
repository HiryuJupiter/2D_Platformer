using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-9000)]
public class GameSettings : MonoBehaviour
{
    public static GameSettings instance { get; private set; }
    
    [Header("Player Movement")]
    [Range(0.1f, 4f)] [SerializeField] float steerSpeedGround = 1f; //50f
    [Range(0.1f, 6f)] [SerializeField] float steerSpeedAir = 4f; //50f
    [SerializeField] float playerMoveSpeed = 10;

    public float SteerSpeedGround => steerSpeedGround;
    public float SteerSpeedAir => steerSpeedAir;
    public float PlayerMoveSpeed => playerMoveSpeed;

    [Header("Normal Jump")]
    [SerializeField] float minJumpForce = 12f;
    [SerializeField] float maxJumpForce = 22f;
    public float MinJumpForce => minJumpForce;
    public float MaxJumpForce => maxJumpForce;

    [Header("Wall Jump")]
    [SerializeField] Vector2 wallJumpUpForce = new Vector2(20f, 22f); //7.5, 16
    [SerializeField] Vector2 wallJumpNormal = new Vector2(15f, 7f); //8.5, 7
    [SerializeField] Vector2 wallJumpAwayForce = new Vector2(22f, 22f); //18, 17
    [SerializeField] Vector2 wallDetachForce = new Vector2(8.5f, 7f); //18, 17
    [SerializeField] float wallStickMaxDuration = .25f;
    [SerializeField] float wallSlideSpeed = 3;
    public Vector2 WallJumpClimbUp => wallJumpUpForce;
    public Vector2 WallJumpNormal => wallJumpNormal;
    public Vector2 WallJumpAway => wallJumpAwayForce;
    public Vector2 WallDetachForce => wallDetachForce;
    public float WallStickMaxDuration => wallStickMaxDuration;
    public float WallSlideSpeed => wallSlideSpeed;


    [Header("Gravity")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float maxFallSpeed = -15f;
    [SerializeField] float gravity = 80f;
    public LayerMask GroundLayer => groundLayer;
    public float MaxFallSpeed => maxFallSpeed;
    public float Gravity => gravity;


    [Header("Physics  behavior")]
    [SerializeField] bool stickyGround = true;
    [SerializeField] int maxSlopeAngle = 70;

    public bool StickyGround => stickyGround;
    public int MaxSlopeAngle => maxSlopeAngle;

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