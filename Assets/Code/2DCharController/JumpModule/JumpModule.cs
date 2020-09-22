using UnityEngine;

public abstract class JumpModule: ScriptableObject
{
    protected Player2DController_Motor motor;

    public void Initialize(Player2DController_Motor motor)
    {
        this.motor = motor;
    }

    public abstract void OnBtnDown();
    public abstract void OnBtnHold();
    public abstract void OnBtnUp();

}
