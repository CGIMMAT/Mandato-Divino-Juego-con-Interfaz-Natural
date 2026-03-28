using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponData : ItemData
{
    public int durability; //Usos antes de romperse
    public string attackType; //Tipo de ataque
    public int damage; //Daño
    public float attackRange; //Rango en casillas
    public float attackSpeed; //Tiempo que tardan en recargar el ataque
}