using UnityEngine;
using System.Collections;


public static class SceneEvents
{
    public static SceneEvent GameStart { get; private set; } = new SceneEvent("Gamestart");

    public static SceneEvent PlayerSpawn { get; private set; } = new SceneEvent("PlayerSpawn");
    public static SceneEvent PlayerDead { get; private set; } = new SceneEvent("PlayerDead");
    public static SceneEvent GameQuit { get; private set; } = new SceneEvent("GameQuit");

    //Game wide event
    public static SceneEvent GameSave { get; private set; } = new SceneEvent("GameSave");
    public static SceneEvent GameLoad { get; private set; } = new SceneEvent("GameLoad");

    public static bool GameWideEventsInitialized { get; private set; }
    public static bool PerLevelEventsInitialized { get; private set; }

    public static void UnSubscribePerLevelEvents ()
    {
        GameStart.UnSubscribeAll();
        PlayerSpawn.UnSubscribeAll();
        PlayerDead.UnSubscribeAll();
        GameQuit.UnSubscribeAll();
    }
}