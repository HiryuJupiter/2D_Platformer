using UnityEngine;
using System.Collections;

public class Coin : Interactables
{
    //WHen player touches coin (by checking againt the gameobject layer), tell the scene manager you've picked up a coin.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameSettings.instance.PlayerLayer == 1 << collision.gameObject.layer)
        {
            Debug.Log("touched coin");
            GameplaySceneManager.instance.PickUpCoin();
            Destroy(gameObject);
        }
    }
}