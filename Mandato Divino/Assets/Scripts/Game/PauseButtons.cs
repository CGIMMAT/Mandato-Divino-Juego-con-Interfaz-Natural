using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    public ResourceManager RM;

    public void Forfeit()
    {
        if (DataKeeper.instance != null)
        {
            DataKeeper.instance.FinalTime = RM.time;
        }

        SceneManager.LoadScene("EndScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
