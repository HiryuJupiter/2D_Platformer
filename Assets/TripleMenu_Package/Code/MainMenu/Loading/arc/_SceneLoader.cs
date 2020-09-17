using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class _SceneLoader : MonoBehaviour
{
    //Issues with synchronous loading: no way of knowing when it's done. 
    //Synchornous loading locks up the main thread, where everything freezes until the scene is loaded, 
    //including your loading screen.

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");

        StartCoroutine(LoadAfterTimer());
    }

    IEnumerator LoadAfterTimer()
    {
        // the reason we use a coroutine is simply to avoid a quick "flash" of the 
        // loading screen by introducing an artificial minimum load time :boo:
        yield return new WaitForSeconds(2.0f);

        LoadScene("Game");
    }

    private void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}