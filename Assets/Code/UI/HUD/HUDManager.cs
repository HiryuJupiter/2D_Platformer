using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] GameObject HUD_Group;
    [SerializeField] GameObject GameWon;
    [SerializeField] Text coinScore;

    GameplaySceneManager sceneM;

    bool startedTimer = false;

    void Awake()
    {
        instance = this;
        GameWon.SetActive(false);
    }

    void Start()
    {
        //Refeence
        sceneM = GameplaySceneManager.Instance;

        //HUD_Hide();
        SubscribeEvents();

        //Initialize
        coinScore.text = "Coins: 0 / " + sceneM.coinsInScene;
    }

    #region Public - Setting values
    public void SetCoinsScore (int amount)
    {
        coinScore.text = "Coins: " + amount + " / " + sceneM.coinsInScene;
    }

    public void SetHealth(int amount)
    {
        //healthBar.SetHealth(amount);
    }

    public void ShowGameWonScreen ()
    {
        GameWon.SetActive(true);
    }
    #endregion

    #region HUD visibility
    void HUD_Reveal()
    {
        HUD_Group.SetActive(true);
    }

    void HUD_Hide()
    {
        HUD_Group.SetActive(false);
    }
    #endregion

    #region Event subscribing
    void SubscribeEvents()
    {
        //SceneEvents.GameStart.Event += HUD_Reveal;
        //SceneEvents.PlayerDead.Event += HUD_Hide;
    }

    void OnDisable()
    {
        //SceneEvents.GameStart.Event -= HUDInitialization;
        //SceneEvents.PlayerDead.Event -= CloseHUD;
    }
    #endregion
}