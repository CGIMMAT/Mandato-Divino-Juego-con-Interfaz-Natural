using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldResourceLogic : MonoBehaviour //Código para que los prefabs almacenen los datos de sus correspondientes SccriptableObjects
{
    public string resourceName;
    public Resources resourceType;
    public int value;

    protected WorldResourceData data;

    public virtual void Initialize(WorldResourceData resourceData)
    {
        data = resourceData;

        resourceName = data.resourceName;
        resourceType = data.resourceType;
        value = data.value;
    }
}