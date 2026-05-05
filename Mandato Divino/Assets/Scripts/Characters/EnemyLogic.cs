using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyLogic : MonoBehaviour
{
    public EnemyData data; //Datos basicos de los enemigos, que heredarán todas las clases de enemigos

    public abstract void TakeDamage(int amount);
    public abstract bool isDead();
    public abstract Transform GetTransform();
}
