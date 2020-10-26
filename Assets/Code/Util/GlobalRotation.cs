using UnityEngine;
using System.Collections;

//Handles global rotation for things like coins
public class GlobalRotation : MonoBehaviour
{
    public static Quaternion rotation;

    public float CoinRotationModifier = 0.5f;

    void Update()
    {
        rotation = Quaternion.Euler(0f, 0f, Time.time * CoinRotationModifier) ;
    }

    //private void OnGUI()
    //{
    //    //GUI.Label(new Rect(20, 20, 500, 20), "" + );
    //}
}