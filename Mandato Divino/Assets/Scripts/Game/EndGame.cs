using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    void Start()
    {
        if (DataKeeper.instance != null)
        {
            float totalSeconds = DataKeeper.instance.FinalTime;
            float hours = totalSeconds / 60f;
            float days = hours / 24f;

            timeText.text = $"Tu culto ha sobrevivido durante {days:F0} dias y {hours:F0} horas";
        }
    }

    public void EndButton()
    {
        if (DataKeeper.instance != null) Destroy(DataKeeper.instance.gameObject);
        SceneManager.LoadScene("StartScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
