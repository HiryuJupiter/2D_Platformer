using UnityEngine;
using System.Collections;

public abstract class ModuleBase
{
    protected GameSettings settings;
    protected Player2DController_Motor motor;
    protected MotorStatus status;
    protected Player2DRaycaster raycaster;


    public ModuleBase(Player2DController_Motor motor)
    {
        this.motor = motor;

        status = motor.Status;
        raycaster = motor.Raycaster;

        settings = GameSettings.instance;
    }

    public virtual void ModuleEntry() { }
    public virtual void TickFixedUpdate() { }
    public virtual void TickUpdate() { }
    public virtual void ModuleExit() { }


}