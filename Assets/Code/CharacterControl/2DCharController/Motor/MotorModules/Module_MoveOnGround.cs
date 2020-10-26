using UnityEngine;
using System.Collections;

//Handles the logic for horizontal movement
public class Module_MoveOnGround : ModuleBase
{
    public Module_MoveOnGround(Player2DMotor motor, Player2DFeedbacks feedback) : base(motor, feedback)
    { }

    float moveXSmoothDampVelocity;

    float moveSpeed => crawling ? settings.PlayerCrawlSpeed : settings.PlayerMoveSpeed;
    bool crawling;

    public override void ModuleEntry()
    {
        base.ModuleEntry();
        Stand();
    }

    public override void TickUpdate()
    {
        base.TickUpdate();

        feedback.SetFacingBasedOnInput();

        StanceUpdate();
    }

    public override void TickFixedUpdate()
    {
        //Modify x-velocity
        motorStatus.currentVelocity.x = Mathf.SmoothDamp(motorStatus.currentVelocity.x, GameInput.MoveX * moveSpeed, ref moveXSmoothDampVelocity, settings.SteerSpeedGround * Time.deltaTime);
    }

    public override void ModuleExit()
    {
        base.ModuleExit();
        crawling = false;
    }

    void StanceUpdate ()
    {
        if (!crawling && GameInput.PressedDown)
        {
            Crawl();
        }
        else if (crawling && !GameInput.PressedDown)
        {
            Stand();
        }
    }

    void Stand ()
    {
        crawling = false;
        feedback.Animator.PlayOnGround();
    }

    void Crawl ()
    {
        crawling = true;
        feedback.Animator.PlayCrouch();
    }
}