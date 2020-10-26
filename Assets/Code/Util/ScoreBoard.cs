using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] Highscore highscore;
    [SerializeField] GameObject anyKeyToQuit;

    CanvasGroup cvs;

    bool waitingToQuit = false;

    #region MonoBehavior
    private void Awake()
    {
        //Hide canvas
        cvs = GetComponent<CanvasGroup>();
        CanvasGroupHelper.InstantHide(cvs);

        //Hide peripherals
        anyKeyToQuit.SetActive(false);
    }

    void Start()
    {
        //Event subscribing
        EventScribing();
    }

    private void Update()
    {
        if (waitingToQuit && Input.anyKey)
        {
            ToMainMenu();
            waitingToQuit = false;
        }
    }
    #endregion

    #region Public - Button presses
    public void ToReplayGame ()
    {
        //Calls game start event
        SceneEvents.GameStart.CallEvent();
    }

    public void ToMainMenu ()
    {
        //Calls the back to main menu event
        SceneEvents.GameOverBackToMain.CallEvent();
    }
    #endregion


    #region Canvas visibility
    void RevealCanvas()
    {
        //Fade in a canvas and then allow press any key to quit
        StartCoroutine(CanvasGroupHelper.CanvasFadeIn(cvs, 0.1f));
        StartCoroutine(AllowForAnykeyToQuit());
    }

    void HideCanvas()
    {
        StartCoroutine(CanvasGroupHelper.CanvasFadeOut(cvs, 0.1f));
        anyKeyToQuit.SetActive(false);
    }
    #endregion

    #region WaitForKey
    IEnumerator AllowForAnykeyToQuit ()
    {
        //Set a bool that says the player can press a key to quit
        yield return new WaitForSeconds(2f);
        anyKeyToQuit.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        waitingToQuit = true;
        
    }
    #endregion

    #region Event subscribing
    void EventScribing()
    {
        SceneEvents.PlayerDead.OnEvent += RevealCanvas;
        SceneEvents.GameOverBackToMain.OnEvent += HideCanvas;
    }

    void OnDisable()
    {
        SceneEvents.PlayerDead.OnEvent -= RevealCanvas;
        SceneEvents.GameOverBackToMain.OnEvent -= HideCanvas;
    }
    #endregion
}