using UnityEngine;
using System.Collections;

//Test script for checking if player interaction works
public class TestObject : Interactables, IDamagable
{

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Check collided with player
        if (collision.gameObject.layer == GameSettings.instance.PlayerLayer)
        {
            //If player is higher
            if (collision.gameObject.transform.position.y > transform.position.y)
            {
                Destroy(gameObject);

            }
            else
            {
                collision.gameObject.GetComponent<IDamagable>().TakeDamage();
            }
        }
    }

    public void TakeDamage(int dmg = 0)
    {
    }
}