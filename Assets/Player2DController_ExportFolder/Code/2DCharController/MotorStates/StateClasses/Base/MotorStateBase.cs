using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Player2DRaycaster))]
[RequireComponent(typeof(Module_StandardJump))]
public abstract class MotorStateBase
{
    protected GameSettings              settings;
    protected Player2DController_Motor  motor;
    protected MotorStatus               status;
    protected Player2DRaycaster         raycaster;

    protected List<ModuleBase> modules = new List<ModuleBase>();

    public MotorStateBase(Player2DController_Motor motor)
    {
        this.motor      = motor;
        status          = motor.Status;
        raycaster       = motor.Raycaster;
        settings        = GameSettings.instance;
    }

    public virtual void StateEntry()
    {
        foreach (var m in modules)
        {
            m.ModuleEntry();
        }
    }

    public virtual void TickUpdate() 
    {
        foreach (var m in modules)
        {
            m.TickUpdate();
        }
    }

    public virtual void TickFixedUpdate()
    {
        foreach (var m in modules)
        {
            m.TickFixedUpdate();
        }
        Transitions();
    }

    public virtual void StateExit()
    {
        foreach (var m in modules)
        {
            m.ModuleExit();
        }
    }

    protected abstract void Transitions();
}