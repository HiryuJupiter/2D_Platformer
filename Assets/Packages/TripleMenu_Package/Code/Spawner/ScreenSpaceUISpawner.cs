using UnityEngine;
using System.Collections;

public class ScreenSpaceUISpawner : MonoBehaviour
{
    public Transform CanvasParent;
    public GameObject Pf_ui_gradientHealth;
    Vector3 offscreen = new Vector3(-1000f, 0f, 0f);

    public void SpawnHealthBar(float maxHP, Transform objectToFollow)
    {
        GameObject ui = Instantiate(Pf_ui_gradientHealth, offscreen, Quaternion.identity, CanvasParent);
        ui.GetComponent<GradientHealthScreenSpace>().InitializeStats(maxHP, objectToFollow);
    }
}