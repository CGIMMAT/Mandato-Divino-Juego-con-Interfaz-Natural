using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultGenerator : MonoBehaviour
{
    public static DifficultGenerator instance;

    public float dayTime = 1440f;
    public float dayUpgrade = 5;

    public int currentLevel = 1;
    public int maxLevel = 10;

    private float counter;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        counter += Time.deltaTime;

        if (counter >= dayTime * dayUpgrade)
        {
            counter = 0;
            if (currentLevel < maxLevel)
            {
                currentLevel++;
                FindObjectOfType<MonsterSpawner>().UpdateDifficulty(currentLevel);
            }
        }
    }
}
