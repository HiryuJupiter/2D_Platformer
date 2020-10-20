using UnityEngine;
using System.Collections;

public abstract class ModuleBase
{
    protected GameSettings settings;
    protected Player2DMotor motor;
    protected MotorStatus motorStatus;
    protected Player2DFeedbacks feedback;
    protected MotorRaycaster raycaster;


    public ModuleBase(Player2DMotor motor, Player2DFeedbacks feedback)
    {
        this.motor = motor;
        this.feedback = feedback;
        motorStatus = motor.status;
        raycaster   = motor.raycaster;

        settings = GameSettings.instance;
    }

    public virtual void ModuleEntry() { }
    public virtual void TickFixedUpdate() { }
    public virtual void TickUpdate() { }
    public virtual void ModuleExit() { }
}