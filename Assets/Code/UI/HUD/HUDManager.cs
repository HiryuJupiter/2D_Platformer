using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] GameObject UIGroup_GameWon;
    [SerializeField] Text UI_carrotScore;
    [SerializeField] Image HPSlot1;
    [SerializeField] Image HPSlot2;
    [SerializeField] Image HPSlot3;
    [SerializeField] Sprite Sprite_Hp1;
    [SerializeField] Sprite Sprite_Hp2;
    [SerializeField] Sprite Sprite_Hp3;

    bool startedTimer = false;
    GameplaySceneManager sceneM;

    void Awake()
    {
        instance = this;
        UIGroup_GameWon.SetActive(false);
    }

    void Start()
    {
        sceneM = GameplaySceneManager.instance;

        //Initialize
        UI_carrotScore.text = "0";
    }

    private void Update()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetHealth(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetHealth(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetHealth(3);
        }
    }

    #region Public - Setting values
    //Set coin score UI text display
    public void SetCoinsScore (int amount)
    {
        UI_carrotScore.text = amount.ToString() + " / " + sceneM.coinsInScene;
    }

    //Modify the displaying of health point icons
    public void SetHealth(int amount)
    {
        switch (amount)
        {
            case 0:
                HPSlot1.sprite = null;
                HPSlot2.sprite = null;
                HPSlot3.sprite = null;
                break;
            case 1:
                HPSlot1.sprite = Sprite_Hp1;
                HPSlot2.sprite = null;
                HPSlot3.sprite = null;
                break;
            case 2:
                HPSlot1.sprite = Sprite_Hp2;
                HPSlot2.sprite = Sprite_Hp2;
                HPSlot3.sprite = null;
                break;
            case 3:
                HPSlot1.sprite = Sprite_Hp3;
                HPSlot2.sprite = Sprite_Hp3;
                HPSlot3.sprite = Sprite_Hp3;
                break;
        }
    }

    //Reveal the game won screen
    public void ShowGameWonScreen ()
    {
        UIGroup_GameWon.SetActive(true);
    }
    #endregion
}