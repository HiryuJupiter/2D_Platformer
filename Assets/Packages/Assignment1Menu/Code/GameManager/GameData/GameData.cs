using UnityEngine;
using System.Collections;

public class GameData
{
    public int gameLevelIndex;
    //PLAYER
    public int playerLevel;
    public int playerHealth;
    public Vector3 playerPosition;

    #region Save to file & Load from file
    public void SaveGameData()
    {
        gameLevelIndex = GetCurrentSceneIndex;
        SaveSystem.SavePlayerData(this);
    }

    public void LoadGameData(int levelIndex = 0)
    {
        if (!SaveSystem.TryLoadPlayerData(this))
        {
            Debug.LogWarning("Saved game data doesn't exist. Player shouldn't have been able to load in the first place.");

            gameLevelIndex = GetCurrentSceneIndex;
            playerLevel = 1;
            playerHealth = 100;
            playerPosition = new Vector3(0f, 0f, 0f);
        }        
    }

    int GetCurrentSceneIndex => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    #endregion
}