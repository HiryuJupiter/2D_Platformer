using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

//Note: UI text elements must have the same name as the stringkeys.
[RequireComponent(typeof(KeyRemappingUILookup))]
public class KeyRemapper : MonoBehaviour
{
    #region Variables
    [Header("Colors")]
    public Color32 ButtonColor_Default;
    public Color32 ButtonColor_Modifying;
    

    public bool IsListeningForKey { get; private set; }

    //Class reference
    KeyRemappingUILookup Lookup;
    SfxManager sfxManager;
    //KeybindReset resetter;

    //Cache UI elements
    GameObject currentButton;
    Image currentImage;
    Text currentText;
    Color32 TextColor_Default;
    string previousStringOnButton; //Cache the original text so we can revert changes

    //Buffer keybinds: so we can revert the changes
    Dictionary<string, KeyCode> bufferKeybind = new Dictionary<string, KeyCode>();
    #endregion

    #region Public
    public void SaveAllKeybinds()
    {
        foreach (var b in bufferKeybind)
        {
            KeyScheme.SaveKeycodeToPlayerPrefs(b.Key, b.Value);
            //Lookup.GetBtnText(b.Key).text = Lookup.GetKeycode(b.Key).ToString();
        }

        KeyScheme.LoadKeycodesFromPlayerPrefs();
    }

    public void Initialize()
    {
        //Reference
        Lookup = GetComponent<KeyRemappingUILookup>();
        sfxManager = SfxManager.instance;

        //Load keyscheme and update display
        KeyScheme.LoadKeycodesFromPlayerPrefs();
        UpdateAllUiButtonText();
    }

    public void UpdateAllUiButtonText()
    {
        Lookup.GetBtnText(Keystrings.Up).text = KeyScheme.Up.ToString();
        Lookup.GetBtnText(Keystrings.Down).text = KeyScheme.Down.ToString();
        Lookup.GetBtnText(Keystrings.Left).text = KeyScheme.Left.ToString();
        Lookup.GetBtnText(Keystrings.Right).text = KeyScheme.Right.ToString();
        Lookup.GetBtnText(Keystrings.Jump).text = KeyScheme.Jump.ToString();
    }

    public void ExitKeyBind()
    {
        Debug.Log("ExitKeyBind");
        currentImage.color = ButtonColor_Default;
        currentText.color = TextColor_Default;
        IsListeningForKey = false;
    }

    public void ChangeKey(GameObject button)
    {
        if (!IsListeningForKey)
        {
            sfxManager.SpawnUI_4_shake();
            IsListeningForKey = true;

            //Cache references
            currentButton = button;
            currentText = button.GetComponentInChildren<Text>();
            currentImage = button.GetComponent<Image>();
            previousStringOnButton = currentText.text;

            //Visual indication that we're listening for a key input
            currentImage.color = ButtonColor_Modifying;
            currentText.text = "???";
            TextColor_Default = currentText.color;
            currentText.color = Color.white;

            //Begin listening for an input
            StartCoroutine(ListenForKeyInput());
        }

        //resetter.CloseConfirmMenu();
    }

    public void ResetKeys()
    {
        SetBufferKeybind(Keystrings.Up, KeyCode.W);
        SetBufferKeybind(Keystrings.Down, KeyCode.S);
        SetBufferKeybind(Keystrings.Left, KeyCode.A);
        SetBufferKeybind(Keystrings.Right, KeyCode.D);
        SetBufferKeybind(Keystrings.Jump, KeyCode.Space);
    }
    #endregion

    #region Keybind
    IEnumerator ListenForKeyInput()
    {
        while (IsListeningForKey)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) //Exit keybind
            {
                ExitKeyBind();
                currentText.text = previousStringOnButton;
                yield break;
            }

            if (Input.anyKeyDown)
            {
                foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
                {
                    //If player pressed down a valid key.
                    //We use this instead of OnGUI's Event.Current in order to listen for Joypad.Buttons
                    if ((int)keycode < 323 && Input.GetKeyDown(keycode))
                    {
                        //Prevents binding the Quit-button causes to quit the options menu on the same frame.
                        //StartCoroutine(menuM.PreventScreenChangeForOneFrame());

                        //Update UI Text
                        currentText.text = keycode.ToString();

                        //Save (int)keycode to playerPref
                        string keystring = Lookup.GetKeystringOfButton(currentButton);
                        SetBufferKeybind(keystring, keycode);
                        sfxManager.SpawnUI_4_shake();

                        //KeyScheme.SaveKeycodeToPlayerPrefs(keystring, keycode); //If you want to save 1 key at a time

                        //Debug.Log("Remapped " + currentText.name + " button: " + keycode);
                        ExitKeyBind();
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    void SetBufferKeybind(string keystring, KeyCode keyCode)
    {
        bufferKeybind[keystring] = keyCode;

        Lookup.GetBtnText(keystring).text = keyCode.ToString();
        //Lookup.GetBtnText(keystring).text = Lookup.GetKeycode(keystring).ToString();
    }
    #endregion
}