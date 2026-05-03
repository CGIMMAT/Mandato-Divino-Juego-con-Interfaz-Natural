using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsLogic : MonoBehaviour //Para que todoslos edificios inicialicen sus datos comunes en cada prefab individual
{
    public BuildingID buildingID;
    public string buildingName;
    public int lifePoints;
    public int maxLife; //Para cuando e reconstruya que se guarde su vida original
    public Vector2Int size;
    protected bool isBuilt = false;
    public DestroyedData destroyedData;
    public BuildingData originalData;

    public virtual void Initialize(BuildingData data)
    {
        buildingID = data.id;
        buildingName = data.buildingName;
        lifePoints = data.lifePoints;
        maxLife =data.lifePoints;
        size = data.size;
        originalData = data;

        isBuilt = true;
    }

    void DestroyBuilding()
    {
        if (destroyedData == null) return;

        Vector3 pos = transform.position;

        GameObject destroyedGO = Instantiate(destroyedData.destroyedPrefab,pos,Quaternion.identity);
        DestroyedLogic destroyedLogic = destroyedGO.GetComponent<DestroyedLogic>();

        if (destroyedLogic != null)
        {
            destroyedLogic.Initialize(destroyedData, this);
        }

        ResourceGenerator RG = FindObjectOfType<ResourceGenerator>();
        if (RG != null)
        {
            Vector3Int tile = RG.tilemap.WorldToCell(pos);
            RG.RegisterResource(tile, destroyedGO);
        }

        Destroy(gameObject);
    }
}
