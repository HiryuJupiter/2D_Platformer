using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "MotorState_WallClimb", menuName = "Motor States/Wall Climb")]
public class MotorState_WallClimb : MotorModuleBase
{
    [Header("Wall Stick")]
    [SerializeField] float wallSlideSpeed = 3;
    [Tooltip("Allows pressing away from the wall without immediately disconnecting, " +
        "so the player can jump away with a stronger force.")]
    [SerializeField] float wallStickMaxDuration = .25f;

    //Status
    float wallStickTimer;
    bool isWallSliding;

    #region Public 
    public override void StateEntry()
    {
        isWallSliding = true;
        wallStickTimer = wallStickMaxDuration;
    }

    public override void OnFixedUpdate()
    {
        status.wallSign = raycaster.GetWallDirSign();
        if (status.isOnGround)
        {
            motor.GoToState(MotorStates.OnGround);
        }
        else if (status.isMovingUp || status.wallSign == 0)
        {
            motor.GoToState(MotorStates.Aerial);
        }
        else if (status.wallSign != 0)
        {
            WallSliding();
            JumpCheck();
        }
    }

    public override void StateExit()
    {
        isWallSliding = false;
    }
    #endregion

    #region Private
    void WallSliding()
    {
        //Limit sliding speed
        if (status.currentVelocity.y < -wallSlideSpeed)
            status.currentVelocity.y = -wallSlideSpeed;

        //Unstuck delay
        if (wallStickTimer > 0)
        {
            //Fixed x movement while you have wallStick timer.
            status.currentVelocity.x = 0;

            //If not pressing away from the wall, tick the timer toward unsticking.
            if (status.movingSign != 0 && status.movingSign != status.wallSign)
            {
                wallStickTimer -= Time.deltaTime;
            }
            else
            {
                wallStickTimer = wallStickMaxDuration;
            }
        }
    }

    void JumpCheck ()
    {
        if (GameInput.JumpBtnDown)
        {
            jumpModule.WallJump(status.wallSign, status.movingSign);
        }
    }
    #endregion

    void OnGUI ()
    {
        GUI.Label(new Rect(600, 0, 290, 20), "=== WALL CLIMB === ");
        GUI.Label(new Rect(600, 20, 290, 20), "wallSign: " + status.wallSign);
        GUI.Label(new Rect(600, 40, 290, 20), "wallStickTimer: " + wallStickTimer);
        GUI.Label(new Rect(600, 60, 290, 20), "isWallSliding: " + isWallSliding);
    }
}