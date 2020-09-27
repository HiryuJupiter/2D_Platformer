using UnityEngine;

public abstract class JumpModule: ScriptableObject
{
    protected IPlayer2DControllerMotor motor;

    public void Initialize(IPlayer2DControllerMotor motor)
    {
        this.motor = motor;
    }

    public abstract void OnBtnDown();
    public abstract void OnBtnHold();
    public abstract void OnBtnUp();

}
