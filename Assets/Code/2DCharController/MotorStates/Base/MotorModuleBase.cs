using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player2DRaycaster))]
[RequireComponent(typeof(JumpModule))]
public abstract class MotorModuleBase: ScriptableObject
{
    protected Player2DController_Motor  motor;
    protected MotorStatus               status;
    protected Player2DRaycaster         raycaster;
    protected JumpModule                jumpModule;

    //public MotorModuleBase(Player2DController_Motor motor)
    //{
    //    this.motor  = motor;

    //    status          = motor.status;
    //    raycaster       = motor.raycaster;
    //    jumpModule      = motor.jumpModule;
    //}

    public virtual void Initialize (Player2DController_Motor motor)
    {
        this.motor = motor;

        status = motor.status;
        raycaster = motor.raycaster;
        jumpModule = motor.jumpModule;
    }

    public abstract void StateEntry();

    public virtual void OnUpdate() { }
    public abstract void OnFixedUpdate();

    public abstract void StateExit();
}