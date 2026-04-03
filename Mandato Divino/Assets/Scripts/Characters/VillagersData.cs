using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Age //Las posibles edades que puede tener
{
    Child,
    Teen,
    Adult,
    Old
}

public enum Gender //Su genero, que determinara de quien se enamora y su sprite
{
    Male,
    Female
}

[System.Serializable]
public class Relationship //Sistema de seguimiento de las relaciones
{
    public int LoverID; //El identificador del amante
    public bool inLove; //El estado de la relacion
}

[CreateAssetMenu(menuName = "Villager")]
public class VillagersData : ScriptableObject //El código para definir los datos de los aldeanos
{
    public string characterName; //El nombre del personaje
    public int lifePoints; //Su vida
    public int inventorySlots; //Los items que pueden llevar
    public int energyPoints; //La cantidad de accionesque pueden hacer antes de ir a descansar

    public GameObject prefab; //El prefab que se instancia
}