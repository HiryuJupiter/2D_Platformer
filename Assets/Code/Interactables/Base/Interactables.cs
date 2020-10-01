using UnityEngine;

public abstract class Interactables: MonoBehaviour
{
    public virtual void PlayerCollided() { }

    protected void Destroy ()
    {
        Destroy(gameObject);
    }
}