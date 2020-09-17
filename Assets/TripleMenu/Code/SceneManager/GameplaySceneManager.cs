using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameplaySceneManager : Singleton<GameplaySceneManager>
{
    public GameStates GameState { get; protected set; }

    const int MainMenuIndex = 0;

    #region MonoBehavior
    protected void Awake()
    {
        DeleteDuplicateSingleton();
        if (instance == null)
        {
            instance = this;
        }
    }

    void OnDisable()
    {
        SceneEvents.UnSubscribePerLevelEvents();
    }
    #endregion

    #region Public
    public void ReturnToMainMenu ()
    {
        SceneManager.LoadScene(MainMenuIndex);
    }
    #endregion
}