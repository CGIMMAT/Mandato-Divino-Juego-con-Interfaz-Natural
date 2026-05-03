using System.Collections;
using System.Collections.Generic;
using UnityEngine; //Se calculan los recursos que poseen los jugadores

public enum Resources //listados de todos los posibles recursos disponibles
{
    None,
    Agua,
    Comida,
    Madera,
    Piedra,
    Metal,
    Herramientas, //Solo las herramientas
    Armas, //Solo las armas
    Familia, //Recurso especial para las casas, donde una familia se puede almacenar
}

[System.Serializable]
public class Cost //Sistema para controlar lo que cuesta fabricar las cosas
{
    public Resources material;
    public int amount;
}

public enum BuildingID
{
    Altar,
    Casa,
    Pozo,
    Granja,
    Plantacion,
    Serreria,
    Cantera,
    Mina,
    Herreria,
    Cocina,
    AlmacenAgua,
    AlmacenComida,
    AlmacenMadera,
    AlmacenPiedra,
    AlmacenMetal,
    AlmacenHerramientas,
    AlmacenArmas,
}

public abstract class BuildingData: ScriptableObject //Los datos comunes para todos los edificios
{
    public BuildingID id; //Su identificador para cuando sea necesario crearlo
    public string buildingName; //Su nombre para el jugador
    public int lifePoints; //El daño que puede sufrir antes de ser destruido

    public GameObject prefab;

    public List<Cost> productionCost;
    public float productionTime; //Lo que se tarda en fabricar
    public Vector2Int size; //Tamaño en tiles

    public bool needsEmptyTile;
}