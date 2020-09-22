using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Jump Burst", menuName = "Jump Modules/Burst")]
public class JumpModule_Burst : JumpModule
{
    [SerializeField] float minJumpForce = 12f;
    [SerializeField] float maxJumpForce = 22f;


    public override void OnBtnDown()
    {
        motor.SetVelocityY(maxJumpForce);
    }

    public override void OnBtnHold()
    {
    }

    public override void OnBtnUp()
    {
        if (motor.GetVelocity.y > minJumpForce)
        {
            motor.SetVelocityY(minJumpForce);
        }
    }

}