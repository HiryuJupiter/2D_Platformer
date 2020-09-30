using UnityEngine;
using System.Collections;
using UnityEditorInternal;

public class Module_WallClimb : ModuleBase
{
    public Module_WallClimb(Player2DController_Motor motor) : base(motor) { }

    public override void ModuleEntry()
    {
        base.ModuleEntry();
        status.wallStickTimer = settings.WallStickMaxDuration;
    }

    public override void TickFixedUpdate()
    {
        status.wallSign = raycaster.GetWallDirSign();
        if (status.wallSign != 0)
        {
            WallSlide();
        }
    }

    public override void ModuleExit()
    {
        base.ModuleExit();
        status.wallStickTimer = -1;
    }

    void WallSlide()
    {
        //Limit sliding speed
        if (status.currentVelocity.y < -settings.WallSlideSpeed)
            status.currentVelocity.y = -settings.WallSlideSpeed;

        //Unstuck delay
        if (status.wallStickTimer > 0)
        {
            //Fixed x movement while you have wallStick timer.
            status.currentVelocity.x = 0;

            //If not pressing away from the wall, tick the timer toward unsticking.
            if (status.moveSign != 0 && status.moveSign != status.wallSign)
            {
                status.wallStickTimer -= Time.deltaTime;
            }
            else
            {
                status.wallStickTimer = settings.WallStickMaxDuration;
            }
        }
    }
}