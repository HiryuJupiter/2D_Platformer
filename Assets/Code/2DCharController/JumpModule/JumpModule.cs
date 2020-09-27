using UnityEngine;

public abstract class JumpModule: ScriptableObject
{
    protected IPlayer2DControllerMotor motor;

    [SerializeField] Vector2 wallJumpClimbUp  = new Vector2(7.5f, 16f);
    [SerializeField] Vector2 wallJumpNormal   = new Vector2(8.5f, 7f);
    [SerializeField] Vector2 wallJumpAway     = new Vector2(18f, 17f);

    public void Initialize(IPlayer2DControllerMotor motor)
    {
        this.motor = motor;
    }

    public virtual void OnWallJump(int wallSign, int moveSign)
    {
        Vector2 v;
        if (wallSign == moveSign)
        {
            v = wallJumpClimbUp;
        }
        else if (moveSign == 0)
        {
            v = wallJumpNormal;
        }
        else
        {
            v = wallJumpAway;
        }
        v.x *= -wallSign;
        motor.SetVelocity(v);
    }

    public abstract void OnBtnDown();
    public abstract void OnBtnHold();
    public abstract void OnBtnUp();
}
