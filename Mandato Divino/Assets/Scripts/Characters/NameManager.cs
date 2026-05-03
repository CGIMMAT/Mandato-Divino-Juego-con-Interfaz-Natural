using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameManager : MonoBehaviour //Código para asignar nombres a los aldeanos.
{
    public static NameManager Instance; //Se instancia por cada aldeano

    //Listas con todos los nombres que pueden tener los aldeanos
    private List<string> maleNames = new List<string>();
    private List<string> femaleNames = new List<string>();

    void Awake() //Al iniciar, se cargan los nombres si existe una instancia 
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadNames();
    }

    void LoadNames() //Importamos los nombres desde los correspondientes archivos de texto
    {
        TextAsset maleFile = UnityEngine.Resources.Load<TextAsset>("maleName");
        TextAsset femaleFile = UnityEngine.Resources.Load<TextAsset>("femaleName");

        if (maleFile != null)
        {
            maleNames = new List<string>(maleFile.text.Split('\n'));
            maleNames.RemoveAll(name => string.IsNullOrWhiteSpace(name));
        }

        if (femaleFile != null)
        {
            femaleNames = new List<string>(femaleFile.text.Split('\n'));
            femaleNames.RemoveAll(name => string.IsNullOrWhiteSpace(name));
        }
    }

    public List<string> GetAllNames() //Listado de todos los nombres disponibles, que se usarán más adelante
    {
        List<string> allNames = new List<string>();
        allNames.AddRange(maleNames);
        allNames.AddRange(femaleNames);
        return allNames;
    }

    public string GetRandomName(Gender gender) //Ahora, en base al genero, se elige un nombre de la lista del genero correspondiente aleatoriamente
    {
        if (gender == Gender.Hombre && maleNames.Count > 0)
            return maleNames[Random.Range(0, maleNames.Count)];
        if (gender == Gender.Mujer && femaleNames.Count > 0)
            return femaleNames[Random.Range(0, femaleNames.Count)];
        return "Villager";
    }
}