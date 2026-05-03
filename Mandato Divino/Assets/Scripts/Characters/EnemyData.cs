using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : ScriptableObject //Los datos básicos que configura a todos los enemigos
{
    public string enemyName; //Su nombre 
    public int lifePoints; //Su vida
    public int attackDamage; //El daño que hacen al atacar
    public GameObject prefab; //El prefab que se instancia
}
