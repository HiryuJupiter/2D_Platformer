using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientHealthScreenSpace : MonoBehaviour
{
    [Header("References")]
    
    public Image BarFG;
    public Gradient gradient;
    Transform followObj;

    [Header("Stats")]
    public float maxHealth = 100;

    float currentHealth = 100;
    RectTransform rectTransform;

    public void InitializeStats(float maxHealth, Transform followObj)
    {
        rectTransform = GetComponent<RectTransform>();
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.followObj = followObj;
        Debug.Log("InitializeStats");
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    #region Unity
    void Start()
    {
        Debug.Log("gradient hp Start");
        UpdateHealthBar();
    }


    void Update()
    {
        FollowTarget();

        //Debug
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(5);
        }
    }
    #endregion

    #region Health bar
    void UpdateHealthBar()
    {
        BarFG.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        BarFG.color = gradient.Evaluate(BarFG.fillAmount);
    }

    void FollowTarget()
    {
        //This is working
        Vector3 worldPos = Camera.main.WorldToScreenPoint(followObj.position);
        rectTransform.transform.position = worldPos;
    }
    #endregion
}
