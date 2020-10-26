using UnityEngine;

//Base class for objects that are interactable
public abstract class Interactables: MonoBehaviour
{
    public virtual void PlayerCollided() { }

    protected void Destroy ()
    {
        Destroy(gameObject);
    }
}