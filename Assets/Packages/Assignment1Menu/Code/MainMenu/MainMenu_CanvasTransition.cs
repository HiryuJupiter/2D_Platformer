using UnityEngine;
using System.Collections;

public class MainMenu_CanvasTransition : MonoBehaviour
{
    [Range(0f, 1f)]
    public float TransitionDuration = 0.2f;

    [SerializeField] CanvasGroup Canvas_PressAnyKey;
    [SerializeField] CanvasGroup Canvas_MainMenu;
    [SerializeField] CanvasGroup Canvas_OptionsMenu;
    [SerializeField] CanvasGroup Canvas_AboutMenu;
    [SerializeField] CanvasGroup Canvas_LoadingScreen;


    SfxManager sfxManager;

    bool inSplash = false;

    #region Initialization
    void Awake()
    {
        CanvasGroupHelper.InstantHide(Canvas_MainMenu);
        CanvasGroupHelper.InstantHide(Canvas_OptionsMenu);
        CanvasGroupHelper.InstantHide(Canvas_AboutMenu);
        CanvasGroupHelper.InstantHide(Canvas_LoadingScreen);
        StartCoroutine(ShowSplashScreen());
    }

    private void Start()
    {
        sfxManager = SfxManager.instance;
    }

    IEnumerator ShowSplashScreen()
    {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(CanvasGroupHelper.CanvasFadeIn(Canvas_PressAnyKey, TransitionDuration));
        yield return new WaitForSeconds(TransitionDuration);
        inSplash = true;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (inSplash && Input.anyKeyDown)
        {
            inSplash = false;
            //sfxManager.SpawnUIMenuTransition();
            SplashToMain();
        }
    }
    #endregion

    #region Public - transition call
    public void SplashToMain()
    {
        PlaySfx();
        StartCoroutine(CanvasGroupHelper.CanvasesCrossfade(Canvas_PressAnyKey, Canvas_MainMenu, TransitionDuration));
    }

    public void MainToOptions()
    {
        PlaySfx();
        StartCoroutine(CanvasGroupHelper.CanvasesCrossfade(Canvas_MainMenu, Canvas_OptionsMenu, TransitionDuration));
    }

    public void OptionsToMain()
    {
        PlaySfx();
        StartCoroutine(CanvasGroupHelper.CanvasesCrossfade(Canvas_OptionsMenu, Canvas_MainMenu, TransitionDuration));
    }

    public void MainToAbout()
    {
        PlaySfx();
        StartCoroutine(CanvasGroupHelper.CanvasesCrossfade(Canvas_MainMenu, Canvas_AboutMenu, TransitionDuration));
    }

    public void AboutToMain()
    {
        PlaySfx();
        StartCoroutine(CanvasGroupHelper.CanvasesCrossfade(Canvas_AboutMenu, Canvas_MainMenu, TransitionDuration));
    }

    public void MainToLoading()
    {
        PlaySfx();
        StartCoroutine(CanvasGroupHelper.CanvasesCrossfade(Canvas_MainMenu, Canvas_LoadingScreen, TransitionDuration));
    }
    #endregion

    void PlaySfx ()
    {
        sfxManager.SpawnUI_1_Click();
    }
}