using UnityEngine;

public abstract class JumpModule: ScriptableObject
{
    protected Player2DController_Motor motor;

    public void Initialize(Player2DController_Motor motor)
    {
        this.motor = motor;
    }

    public abstract void StartApplyingJumpForce();

    public abstract void StopApplyingJumpForce();

}
