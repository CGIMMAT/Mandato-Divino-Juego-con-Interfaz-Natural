using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedLogic : BuildingsLogic //Los datos inicializados del scriptable object en el prefab
{
    public DestroyedData data;
    private BuildingData originalBuildingData;

    public void Initialize(DestroyedData d, BuildingsLogic original)
    {
        data = d;
        originalBuildingData = original.originalData;
    }

    public BuildingData GetOriginalBuilding()
    {
        return originalBuildingData;
    }
}
