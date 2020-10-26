using UnityEngine;

public class ParallaxEventCaller : MonoBehaviour
{
    //Event
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

    //Check if this gameobject has changed position, then call the public static event this occured.
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