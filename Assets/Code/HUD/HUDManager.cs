using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] GameObject HUD_Group;
    [SerializeField] CoinScore coinScore;
    

    bool startedTimer = false;
    

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CloseHUD();
        SubscribeEvents();
    }

    void Update()
    {
        if (startedTimer)
        {
        }
    }

    #region Public - Setting values
    public void SetCoins (int amount)
    {
        coinScore.SetCoins(amount);
    }

    public void SetHealth(int amount)
    {
        //healthBar.SetHealth(amount);
    }

    public void ResetTimer()
    {
        //hudTimer.ResetTimer();
    }
    #endregion

    #region HUD visibility
    void HUDInitialization()
    {
        //Reset HUD
        ResetTimer();
        startedTimer = true;

        //Reveal HUD
        HUD_Group.SetActive(true);
    }

    void CloseHUD()
    {
        HUD_Group.SetActive(false);
    }
    #endregion

    #region HUD Timer
    
    #endregion

    #region Event subscribing
    void SubscribeEvents()
    {
        //SceneEvents.GameStart.Event += HUDInitialization;
        //SceneEvents.PlayerDead.Event += CloseHUD;
    }

    void OnDisable()
    {
        //SceneEvents.GameStart.Event -= HUDInitialization;
        //SceneEvents.PlayerDead.Event -= CloseHUD;
    }
    #endregion
}