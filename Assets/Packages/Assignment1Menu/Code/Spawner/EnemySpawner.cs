using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Pf_Enemy;
    

    void Start()
    {
        
    }

    void Update()
    {
        //Debug   
        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnEnemy(transform.position, 100);
        }
    }

    void SpawnEnemy (Vector3 pos, float maxHP)
    {
        GameObject go = Instantiate(Pf_Enemy, pos, Quaternion.identity);
    }
}
