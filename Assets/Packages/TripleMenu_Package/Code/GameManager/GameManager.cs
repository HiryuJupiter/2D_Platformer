using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] 
    public bool loadeSaveFile = false;

    public GameData GameData { get; private set; }

    void Awake()
    {
        //Singleton
        DeleteDuplicateSingleton();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }

        if (loadeSaveFile)
        {
            LoadGameData();
        }
    }

    void LoadGameData ()
    {
        loadeSaveFile = false;
        Debug.Log("GameManager calls SceneEvents.GameLoad");

        SceneEvents.GameLoad.CallEvent();
    }
}
