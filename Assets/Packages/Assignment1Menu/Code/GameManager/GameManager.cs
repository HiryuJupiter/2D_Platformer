using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 

    [HideInInspector] 
    public bool loadeSaveFile = false;

    public GameData GameData { get; private set; }

    void Awake()
    {
        //Singleton
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
