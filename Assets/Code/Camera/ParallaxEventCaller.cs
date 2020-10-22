using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;

    private void Start()
    {
        //Subscribe
        ParallaxEventCaller.OnParallaxMove += Move;
    }

    private void OnDisable()
    {
        //Unsubscribe
        ParallaxEventCaller.OnParallaxMove -= Move;
    }

    public void Move(Vector3 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }
}

[ExecuteInEditMode]
public class ParallaxEventCaller : MonoBehaviour
{
    public delegate void ParallaxMove(Vector3 movement);
    public static event ParallaxMove OnParallaxMove;

    Vector3 prevPosition;

    void Start()
    {
        prevPosition = transform.position;
    }

    void Update()
    {
        PositionalChangeCheck();
    }

    void PositionalChangeCheck ()
    {
        if (transform.position != prevPosition)
        {
            if (OnParallaxMove != null)
            {
                Vector3 delta = prevPosition - transform.position;
                OnParallaxMove(delta);
            }
            prevPosition = transform.position;
        }
    }
}