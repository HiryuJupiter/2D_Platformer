using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameplaySceneManager : Singleton<GameplaySceneManager>
{
    public GameStates GameState { get; protected set; }

    public int coinsInScene { get; private set; }

    HUDManager hud;
    int coinsPickedUp;
    const int MainMenuIndex = 0;

    #region MonoBehavior
    void Awake()
    {
        DeleteDuplicateSingleton();
        if (instance == null)
        {
            instance = this;
        }

        coinsInScene = GameObject.FindObjectsOfType<Coin>().Length;
    }

    private void Start()
    {
        hud = HUDManager.instance;
    }

    void OnDisable()
    {
        SceneEvents.UnSubscribePerLevelEvents();
    }
    #endregion
    
    #region Public
    public void PickUpCoin ()
    {
        hud.SetCoinsScore(++coinsPickedUp);
        if (coinsPickedUp >= coinsInScene)
        {
            GameOver();
        }
    }

    public void ReduceHealth ()
    {

    }

    

    public void ReturnToMainMenu ()
    {
        SceneManager.LoadScene(MainMenuIndex);
    }
    #endregion

    void GameOver ()
    {
        hud.ShowGameWonScreen();
    }
}