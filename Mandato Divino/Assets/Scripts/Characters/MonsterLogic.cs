using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLogic : MonoBehaviour
{
    public string monsterName; //Su nombre
    public int lifePoints; //Su vida
    public int attackDamage; //El daño que hacen al atacar
    public int lifeSpan; //El tiempo que vive

    public void Initialize(MonsterData data)
    {
        monsterName = data.enemyName;
        lifePoints = data.lifePoints;
        attackDamage = data.attackDamage;
        lifeSpan = data.lifeSpan;

        lifeSpan = Random.Range(lifeSpan, (lifeSpan * 5) + 1); //Se aleatoriza la cantidad de tiempo que viven
    }
}