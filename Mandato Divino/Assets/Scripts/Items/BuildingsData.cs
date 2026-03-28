using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resources //listados de todos los posibles recursos disponibles
{
    Water,
    Food,
    Wood,
    Stone,
    Steel,
    Seeds,
    Plants, //Recurso especial para las plantaciones, donde distintos cultivos se puden plantar
    Logs, //Recurso especial que produce la plantación y se puede convertir en madera
    Items, //Recurso especial para la herrería, donde distintas armas y herramientas se pueden fabricar
    Tools, //Solo las herramientas
    Weapons, //Solo las armas
    Family, //Recurso especial para las casas, donde una familia se puede almacenar
}

[System.Serializable]
public class Cost //Sistema para controlar lo que cuesta fabricar las cosas
{
    public Resources material;
    public int amount;
}

public enum BuildingID
{
    None,
    Altar,
    House,
    Well,
    Farm,
    Plantation,
    Sawmill,
    Quarry,
    Mine,
    Blacksmith,
    Kitchen,
    WaterStorage,
    FoodStorage,
    SeedsStorage,
    WoodStorage,
    StoneStorage,
    SteelStorage,
    ToolsStorage,
    WeaponsStorage,
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