using UnityEngine;
using System.Collections;

//A middle man for saving data, since filestream doesnt handle Vector3 and
//quaternion very well when converting to filestream
[System.Serializable]
public class GameDataSerializable
{
    int playerLevel;
    int playerHealth;
    float[] playerPosition;

    public static void ConvertToGameData(GameDataSerializable data, GameData gameData)
    {
        gameData.playerLevel = data.playerLevel;
        gameData.playerHealth = data.playerHealth;

        Vector3 pos = new Vector3(
            data.playerPosition[0],
            data.playerPosition[1],
            data.playerPosition[2]);
        gameData.playerPosition = pos;
    }

    public static GameDataSerializable ConvertGameDataToSerializable(GameData gameData)
    {
        GameDataSerializable data = new GameDataSerializable();
        data.playerLevel = gameData.playerLevel;
        data.playerHealth = gameData.playerHealth;

        data.playerPosition = new float[3];
        data.playerPosition[0] = gameData.playerPosition.x;
        data.playerPosition[1] = gameData.playerPosition.y;
        data.playerPosition[2] = gameData.playerPosition.z;

        return data;
    }
}