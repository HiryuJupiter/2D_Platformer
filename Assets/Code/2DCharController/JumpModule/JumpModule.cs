using UnityEngine;

[CreateAssetMenu(fileName = "Jump Burst", menuName = "Jump Modules/Burst")]

public class JumpModule : ScriptableObject
{
    Player2DController_Motor motor;
    MotorStatus status;

    [SerializeField] Vector2 wallJumpClimbUp = new Vector2(7.5f, 16f);
    [SerializeField] Vector2 wallJumpNormal = new Vector2(8.5f, 7f);
    [SerializeField] Vector2 wallJumpAway = new Vector2(18f, 17f);

    public void Initialize(Player2DController_Motor motor)
    {
        this.motor = motor;
        status = motor.status;
    }

    public void WallJump(int wallSign, int moveSign)
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
        status.currentVelocity = v;
    }

    [SerializeField] float minJumpForce = 12f;
    [SerializeField] float maxJumpForce = 22f;


    public void NormalJump_OnButtonDown()
    {
        status.currentVelocity.y = maxJumpForce;
    }


    public void NormalJump_OnButtonUp()
    {
        if (status.currentVelocity.y > minJumpForce)
        {
            status.currentVelocity.y = minJumpForce;
        }
    }
}
