using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Se usará para poder modificar los contadores de recursos en la interfaz

public class ResourceManager : MonoBehaviour //Usaremos esta clase como almacen de los recursos del jugador
{
    public int food = 100;
    public int water = 100;
    public int wood = 100;
    public int stone = 100;
    public int seeds = 100;
    public int steel = 100;
    public float time = 0f; //Contará los días de juego 
    public int population = 0;

    public TextMeshProUGUI foodCounter;
    public TextMeshProUGUI waterCounter;
    public TextMeshProUGUI woodCounter;
    public TextMeshProUGUI stoneCounter;
    public TextMeshProUGUI seedsCounter;
    public TextMeshProUGUI steelCounter;
    public TextMeshProUGUI timeCounter;
    public TextMeshProUGUI populationCounter;

    void Update() //La interfaz cuenta en todo momento los recursos que nos quedan
    {
        time += Time.deltaTime;
        float hours = time/60f; //Una hora dentro de la partida equivaldrá a 60 segundos
        float days = hours/24f; //Un día de juego son 24 minutos reales

        foodCounter.text = "Comida: " + food;
        waterCounter.text = "Agua: " + water;
        seedsCounter.text = "Semillas: " + seeds;
        woodCounter.text = "Madera: " + wood;
        stoneCounter.text = "Piedra: " + stone;
        steelCounter.text = "Metales: " + steel;
        populationCounter.text = "Siervos: " + population;
        timeCounter.text = "Día " + days.ToString("F0") + " Hora " + hours.ToString("F0");
    }
}
