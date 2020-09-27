using UnityEngine;
using System.Collections;

public interface IPlayer2DControllerMotor
{
    void SetVelocityY(float velocityY);

    Vector3 GetVelocity();
}