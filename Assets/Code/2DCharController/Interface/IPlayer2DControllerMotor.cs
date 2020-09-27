using UnityEngine;
using System.Collections;

public interface IPlayer2DControllerMotor
{
    void SetVelocity(Vector2 velcity);

    void SetVelocityY(float velocityY);
    Vector3 GetVelocity();
}