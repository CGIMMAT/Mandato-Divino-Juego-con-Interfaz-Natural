using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TemporaryResourceType
{
    Matojo,
    Animal
}

[CreateAssetMenu(menuName = "Resources/TemporaryResource")]
public class TemporaryResourcesData : ScriptableObject //Se define el funcionamiento de los recursos temporales,
{
    public string resourceName; //Su nombre
    public Resources resourceType; //El recurso que dan
    public int value; //La cantidad que dan
    public GameObject resourcePrefab; //el prefab con el que trabajarán
    public TemporaryResourceType resourceKind; //El tipo de recurso que es, que se usará a la hora de la recolección
}