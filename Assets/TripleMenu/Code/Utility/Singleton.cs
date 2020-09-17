using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance { get { return instance; } }

    protected void DeleteDuplicateSingleton()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            //Debug.Log("Duplicate " + typeof(T).ToString() + " has being destroyed.");
        }
    }

    /*Usage
     * protected void Awake()
    {
        DeleteDuplicateSingleton();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }*/
}