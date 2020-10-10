using UnityEngine;
using System.Collections;

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
        crawling = false;
    }

    public override void TickUpdate()
    {
        base.TickUpdate();
        feedback.SetFacingBasedOnInput();

        if (!crawling && GameInput.MoveY < -0.1f)
        {
            crawling = true;
        }
        else if (crawling && GameInput.MoveY > 0.1f)
        {
            crawling = false;
        }
    }

    public override void TickFixedUpdate()
    {
        //Move character
        motorStatus.currentVelocity.x = Mathf.SmoothDamp(motorStatus.currentVelocity.x, GameInput.MoveX * moveSpeed, ref moveXSmoothDampVelocity, settings.SteerSpeedGround * Time.deltaTime);
    }
}