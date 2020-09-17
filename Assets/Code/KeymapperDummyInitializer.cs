using UnityEngine;
using System.Collections;

public class KeymapperDummyInitializer : MonoBehaviour
{
    void Awake()
    {
        KeyScheme.LoadKeycodesFromPlayerPrefs();
    }
}