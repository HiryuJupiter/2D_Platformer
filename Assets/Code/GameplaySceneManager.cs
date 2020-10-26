using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameplaySceneManager : MonoBehaviour
{
    public static GameplaySceneManager instance;

    public GameStates GameState { get; protected set; }

    public int coinsInScene { get; private set; }

    HUDManager hud;
    int coinsPickedUp;
    const int MainMenuIndex = 0;

    #region MonoBehavior
    void Awake()
    {
        instance = this;
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
    //Public method that receives the event for when the player picks up a coin
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
        //Load main menu 
        SceneManager.LoadScene(MainMenuIndex);
    }
    #endregion

    //Game over, tell HUD to reveal game score screen
    void GameOver ()
    {
        hud.ShowGameWonScreen();
    }
}