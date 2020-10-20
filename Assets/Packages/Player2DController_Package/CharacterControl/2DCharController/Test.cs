using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


enum AttackType
{
    Melee,
    Ranged
}

public class Test : MonoBehaviour
{
    public LayerMask layerMask;

    //Component reference
    Collider2D col;
    Rigidbody2D rb;


    int numHits;

    RaycastHit2D[] hits = new RaycastHit2D[6];
    ContactFilter2D filter;

    private void Awake()
    {
        //Reference
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        //Set up Contact filter
        filter.useTriggers = false;
        //filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        filter.SetLayerMask(layerMask);
        filter.useLayerMask = true;
        //Debug.Log(1 << filter.layerMask);

        Collider2D[] collidersArray = new Collider2D[5];
        hits = new RaycastHit2D[6];
    }

    void Update()
    {
        ContactFilterMethod();
    }

    void ContactFilterMethod ()
    {
        numHits = rb.Cast(Vector3.down, filter, hits);
        Debug.DrawRay(transform.position, Vector3.down * 30f, Color.red);

        for (int i = 0; i < numHits; i++)
        {
            Vector2 currentNormal = hits[i].normal;
            Debug.DrawRay(hits[i].point, hits[i].normal);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 500, 20), "numHits" + numHits);

        if (numHits > 0)
        {
            GUI.Label(new Rect(20, 60, 500, 20), "hit 0 " + hits[0].collider.gameObject.name);
        }

        GUI.Label(new Rect(20, 100, 500, 20), "filter.layerMask" + (int)filter.layerMask);

        

    }
}
