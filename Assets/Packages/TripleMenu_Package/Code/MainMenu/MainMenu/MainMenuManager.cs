
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

[RequireComponent(typeof(SceneLoader))]
public class MainMenuManager : MonoBehaviour
{
    SceneLoader sceneLoader;
    const int SceneOne = 1;

    //MainMenuState state = MainMenuState.MainMenu;

    //Class reference
    GameManager gm;
    SfxManager sfxManager;

    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
        sfxManager = SfxManager.instance;

        gm = GameManager.Instance;
    }

    #region Public - Main menu
    public void StartNewGame()
    {
        sceneLoader.LoadLevel(SceneOne);
    }

    public void ContinueGame()
    {
        gm.loadeSaveFile = true;
        gm.GameData.LoadGameData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(gm.GameData.gameLevelIndex);
    }

    public void Clicked_Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
    #endregion
}