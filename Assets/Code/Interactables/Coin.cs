using UnityEngine;
using System.Collections;

public class Coin : Interactables
{

    void Start()
    {

    }

    void Update()
    {

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("OnCollisionEnter2D ??" + collision.gameObject);

    //}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameSettings.instance.PlayerLayer == 1 << collision.gameObject.layer)
        {
            Debug.Log("touched coin");
            GameplaySceneManager.Instance.PickUpCoin();
            Destroy(gameObject);
        }
    }
}