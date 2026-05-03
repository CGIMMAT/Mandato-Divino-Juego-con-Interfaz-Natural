using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Destroyed")]
public class DestroyedData : ScriptableObject //Datos genericos para los edificios destruidos
{
    public GameObject destroyedPrefab; //Su prefab, que sirve mayormente de decoración

    public List<Cost> rebuildCost; //Lo que cuesta de reparar
    public float rebuildTime; //LO que cuesta reconstruirlos

    public BuildingData originalBuilding; //El edificio original
}
