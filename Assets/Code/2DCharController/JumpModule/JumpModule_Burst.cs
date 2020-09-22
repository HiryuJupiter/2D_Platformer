using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Jump Burst", menuName = "Jump Modules/Burst")]
public class JumpModule_Burst : JumpModule
{
    [SerializeField] float minJumpForce = 12f;
    [SerializeField] float maxJumpForce = 22f;


    public override void StartApplyingJumpForce()
    {
        motor.SetVelocityY(maxJumpForce);
    }

    public override void StopApplyingJumpForce()
    {
        if (motor.GetVelocity.y > minJumpForce)
        {
            motor.SetVelocityY(minJumpForce);
        }
    }

}