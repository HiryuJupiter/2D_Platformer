using UnityEngine;

public class ParallaxEventListener : MonoBehaviour
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

    //Check if the parallax event caller has moved position.
    void Move(Vector3 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos -= delta * parallaxFactor;
        transform.localPosition = newPos;
    }
}
