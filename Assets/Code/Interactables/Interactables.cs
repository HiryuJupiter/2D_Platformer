using UnityEngine;

public abstract class Interactables: MonoBehaviour
{
    public float baseMoveSpeed = -5f;

    public virtual void PlayerCollided() { }

    protected void Destroy ()
    {
        Destroy(gameObject);
    }
}