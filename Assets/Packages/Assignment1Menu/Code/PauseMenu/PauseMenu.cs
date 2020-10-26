using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PauseMenuCanvasTransition))]
public class PauseMenu : MonoBehaviour
{
    PauseMenuCanvasTransition transition;
    GameplaySceneManager sceneManager;
    SfxManager sfxManager;
    PauseMenuStates state = PauseMenuStates.Unpaused;

    #region MonoBehavior
    void Start()
    {
        transition = GetComponent<PauseMenuCanvasTransition>();
        sceneManager = GameplaySceneManager.instance;
        sfxManager = SfxManager.instance;

        UnPause();
    }
    void Update()
    {
        //Hotkey controls over options menu
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            switch (state)
            {
                case PauseMenuStates.Unpaused:
                    Pause();
                    break;
                case PauseMenuStates.PauseMain:
                    UnPause();
                    break;
                case PauseMenuStates.OptionsMenu:
                    OptionsToPauseMain();
                    break;
            }
            sfxManager.SpawnUI_1_Click();
        }
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region Public - Click unity events
    public void UIHandle_UnPause()
    {
        sfxManager.SpawnUI_1_Click();
        UnPause();
    }

    public void UIHandle_OpenOptionsMenu ()
    {
        sfxManager.SpawnUI_1_Click();
        PauseMainToOptions();
    }

    public void UIHandle_CloseOptionsMenu ()
    {
        sfxManager.SpawnUI_1_Click();
        OptionsToPauseMain();
    }

    public void UIHandle_QuitGame()
    {
        sfxManager.SpawnUI_1_Click();
        QuitGame();
    }
    #endregion

    #region Pause logic
    void Pause()
    {
        state = PauseMenuStates.PauseMain;
        transition.Pause();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void PauseMainToOptions()
    {
        state = PauseMenuStates.OptionsMenu;
        transition.PauseMainToOptions();
    }

    void OptionsToPauseMain()
    {
        state = PauseMenuStates.PauseMain;
        transition.OptionsToPauseMain();
    }

    void UnPause()
    {
        state = PauseMenuStates.Unpaused;
        transition.UnPause();
        Time.timeScale = 1f;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void QuitGame()
    {
        sceneManager.ReturnToMainMenu();
        UnPause();
    }
    #endregion
}