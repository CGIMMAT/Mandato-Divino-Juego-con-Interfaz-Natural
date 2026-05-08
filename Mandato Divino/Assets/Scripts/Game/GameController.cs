using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public ResourceManager RM;
    public VillagerSpawner VS;

    void Update()
    {
        if (VS.allVillagers.Count <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (DataKeeper.instance != null)
        {
            DataKeeper.instance.FinalTime = RM.time;
        }

        SceneManager.LoadScene("EndScene");
    }
}
