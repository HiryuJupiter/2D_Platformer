using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player2DController_Motor))]
public class Player : MonoBehaviour, IDamagable
{
    Player2DController_Motor motor;

    void Start()
    {

    }

    void Update()
    {

    }

    public void TakeDamage(int dmg = 0)
    {
        //Motor, do knockback
    }
}