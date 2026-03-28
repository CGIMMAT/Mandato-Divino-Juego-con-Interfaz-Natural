using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Storage")]
public class StorageData : BuildingData //Datos especificos para crear almacenes
{
    public Resources storedMaterial; //Lo que guardan
    public int storageCapacity; //La cantidad que guardan
}