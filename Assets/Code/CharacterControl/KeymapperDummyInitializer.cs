using UnityEngine;
using System.Collections;

//THis is  debug class used in scenes that don't have a GameManager, and you just want to 
//get the keycontrol working.
public class KeymapperDummyInitializer : MonoBehaviour
{
    void Awake()
    {
        KeyScheme.LoadKeycodesFromPlayerPrefs();
    }
}