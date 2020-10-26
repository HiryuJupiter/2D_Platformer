using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;
    [SerializeField] 
    GameObject sfx_UI_1_Click;
    [SerializeField]
    GameObject sfx_UI_2_bom;
    [SerializeField]
    GameObject sfx_UI_3_tape;
    [SerializeField]
    GameObject sfx_UI_4_shake;

    bool canPlaySfx = false;

    #region MonoBehavior
    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        //Delay canPlaySfx, because the on start, LoadOptionsSettings will change the 
        //options menu item's values and cause sfx to play.
        yield return new WaitForSeconds(0.2f);
        canPlaySfx = true;
    }
    #endregion

    #region Public
    public void SpawnUI_1_Click ()
    {
        if (canPlaySfx)
            instance.SpawnSfxPrefab(sfx_UI_1_Click);
    }

    public void SpawnUI_2_bom()
    {
        if (canPlaySfx)
            instance.SpawnSfxPrefab(sfx_UI_2_bom);
    }

    public void SpawnUI_3_tape()
    {
        if (canPlaySfx)
            instance.SpawnSfxPrefab(sfx_UI_3_tape);
    }

    public void SpawnUI_4_shake()
    {
        if (canPlaySfx)
            instance.SpawnSfxPrefab(sfx_UI_4_shake);
    }
    #endregion

    #region Private
    void SpawnSfxPrefab (GameObject pf)
    {
        Instantiate(pf, Vector3.zero, Quaternion.identity);
    }
    #endregion
}