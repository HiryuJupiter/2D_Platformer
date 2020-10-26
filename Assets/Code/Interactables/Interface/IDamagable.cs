using UnityEngine;
using System.Collections;

//Interface for taking damage
public interface IDamagable
{
    void TakeDamage(int dmg = 0);
}