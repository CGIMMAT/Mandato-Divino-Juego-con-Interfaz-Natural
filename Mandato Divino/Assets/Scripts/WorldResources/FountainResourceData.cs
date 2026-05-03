using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/FountainResource")]
public class FountainResourceData : ScriptableObject //Datos de las fuentes de recursos
{
    public string resourceName; //Su nombre
    public Resources resourceType; //El tipo de recurso
    public GameObject resourcePrefab; //El prefab con el que se trabaja
    public int life; //La vida que tiene
    public int resourcesPeLife; //La cantidad de recursos por punta de vida perdido que dan
    public int recoveryTime; //El tiempo para que pueda volver a extraerse recursos de la fuente
}
