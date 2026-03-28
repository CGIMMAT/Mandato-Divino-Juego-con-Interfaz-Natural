using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject //Datos genericos para todos los items que no son recursos
{
    public string itemName; //Nombre del item
    // public Sprite image; // Su imagen, de momento no tendrán 
    public List<Cost> productionCost; //Lo que cuestan
    public float productionTime; //Lo que se tarda
}