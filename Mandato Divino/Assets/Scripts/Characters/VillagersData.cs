using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Age //Las posibles edades que puede tener
{
    Niño,
    Joven,
    Adulto,
    Anciano
}

public enum Gender //Su genero, que determinara de quien se enamora y su sprite
{
    Hombre,
    Mujer
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

    public int attackDamage; //El daño que hacen los aldeanos al atacar
    public int recolectDamage; //El daño que hacen a fuentes de recursos al recolectar recursos

    public GameObject prefab; //El prefab que se instancia

    public AudioClip recolectSound; //Efecto desonido que contarán cuando recolecten recursos
    public AudioClip combatSound; //Combatan
    public AudioClip cookSound; //Y cocinen
}