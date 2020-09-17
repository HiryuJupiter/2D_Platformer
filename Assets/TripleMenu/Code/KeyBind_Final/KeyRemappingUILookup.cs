using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/*
We use StringkeyInput to associate individual: 
a. UI buttons that the player press on to start rebinding a key
b. playerPref 
 */
public class KeyRemappingUILookup : MonoBehaviour
{
    //We want the player to drag in the ui-buttons to avoid the risk they'll drag in the ui-label or null root gameobject,
    //as we'll be using the Button's gameobject to referencing and assigning the button-text.
    public Button Up;
    public Button Down;
    public Button Left;
    public Button Right;
    public Button Jump;

    public class UILookUp
    {
        internal string keystring;
        internal GameObject buttonGameObject;
        internal Text buttonText;

        public UILookUp(string keystring, GameObject buttonGameObject, Text buttonText)
        {
            this.keystring = keystring;
            this.buttonGameObject = buttonGameObject;
            this.buttonText = buttonText;
        }
    }

    List<UILookUp> lookups;

    void Awake()
    {
        //Look up dictionary initializations
        lookups = new List<UILookUp>()
            {
                new UILookUp(Keystrings.Up,     Up.gameObject,      Up.GetComponentInChildren<Text>()),
                new UILookUp(Keystrings.Down ,  Down.gameObject,    Down.GetComponentInChildren<Text>()),
                new UILookUp(Keystrings.Left,   Left.gameObject,    Left.GetComponentInChildren<Text>()),
                new UILookUp(Keystrings.Right , Right.gameObject,   Right.GetComponentInChildren<Text>()),
                new UILookUp(Keystrings.Jump,   Jump.gameObject,    Jump.GetComponentInChildren<Text>())
            };
    }

    #region Public - Look ups & getter methods
    public Text GetBtnText(string stringkey)
    {
        return lookups.FirstOrDefault(x => x.keystring == stringkey).buttonText;
    }

    public string GetKeystringOfButton(GameObject target)
    {
        return lookups.FirstOrDefault(x => x.buttonGameObject == target).keystring;
    }

    #endregion
}