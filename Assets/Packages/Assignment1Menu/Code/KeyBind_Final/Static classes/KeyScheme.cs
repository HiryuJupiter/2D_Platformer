using UnityEngine;
using System.Collections;
using System.Security.Principal;

/*
KeyScheme is the finalized KeyCodes that the game will use to directly access for gameplay.
We use a (reference type) class to wrap the (value type) KeyCodes, so that we can store the pointers in a look up dictionary
that is used during KeyRemapping.
 */

public static class KeyScheme
{
    public static KeyCode Up;
    public static KeyCode Down;
    public static KeyCode Left;
    public static KeyCode Right;
    public static KeyCode Jump;

    public static void LoadKeycodesFromPlayerPrefs()
    {
        Up = (KeyCode)PlayerPrefs.GetInt(Keystrings.Up, (int)KeyCode.W);
        Down = (KeyCode)PlayerPrefs.GetInt(Keystrings.Down, (int)KeyCode.S);
        Left = (KeyCode)PlayerPrefs.GetInt(Keystrings.Left, (int)KeyCode.A);
        Right = (KeyCode)PlayerPrefs.GetInt(Keystrings.Right, (int)KeyCode.D);
        Jump = (KeyCode)PlayerPrefs.GetInt(Keystrings.Jump, (int)KeyCode.Space);
    }

    //public static void SaveAllToPlayerPrefs()
    //{
    //    PlayerPrefs.SetInt(Keystrings.Up, (int)Up);
    //    PlayerPrefs.SetInt(Keystrings.Down, (int)Down);
    //    PlayerPrefs.SetInt(Keystrings.Left, (int)Left);
    //    PlayerPrefs.SetInt(Keystrings.Right, (int)Right);
    //    PlayerPrefs.SetInt(Keystrings.Jump, (int)Jump);
    //}

    public static void SaveKeycodeToPlayerPrefs(string stringkey, KeyCode keyCode)
    {
        PlayerPrefs.SetInt(stringkey, (int)keyCode);
    }

    public static void ResetAll()
    {
        PlayerPrefs.SetInt(Keystrings.Up, (int)KeyCode.W);
        PlayerPrefs.SetInt(Keystrings.Down, (int)KeyCode.S);
        PlayerPrefs.SetInt(Keystrings.Left, (int)KeyCode.A);
        PlayerPrefs.SetInt(Keystrings.Right, (int)KeyCode.D);
        PlayerPrefs.SetInt(Keystrings.Jump, (int)KeyCode.Space);

        LoadKeycodesFromPlayerPrefs();
    }
}