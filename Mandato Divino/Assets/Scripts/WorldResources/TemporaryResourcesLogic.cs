using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryResourcesLogic : MonoBehaviour //Código para los prefabs de los recursos temporales
{
    public string resourceName;
    public Resources resourceType;
    public int value;
    protected TemporaryResourcesData data;
    public TemporaryResourceType resourceKind;

    public virtual void Initialize(TemporaryResourcesData resourceData)
    {
        data = resourceData;

        resourceName = data.resourceName;
        resourceType = data.resourceType;
        value = data.value;
        resourceKind = data.resourceKind;
    }
}
