using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemsData : ScriptableObject //Datos genericos para todos los objetos del juego
{
    public string itemName; //El nombre
    public Sprite icon; //El sprite

    public bool isStackable; //Si es un objeto que se puede acumular
    public int maxStack; //su numero máximo
    public Resources resourceType; //EL tipo de item que es y por tanto donde se debe almacenar
}