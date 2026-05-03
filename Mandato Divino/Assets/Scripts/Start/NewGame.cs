using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public void StartGame() //Solamente se cambia a la escena con el mapa
    {
        SceneManager.LoadScene("MapScene");
    }
}
