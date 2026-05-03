using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BuildingRequirements //Sistema para controlar donde se consruyen los edificios
{
    public BuildingID id; //Identifica el tipo de edificio
    public Resources requiredResourceType; //Indicaque tipo de recurso debe haber para poder construir el edificio
}

public class BuildingPlacer : MonoBehaviour //Código que gestiona la aparición de edificios en el juego intercambiando recursos por estos
{
    public Tilemap tilemap;
    public ResourceGenerator RG;
    public ResourceManager RM;

    public List<BuildingData> buildingData;
    public List<BuildingRequirements> buildingRequeriments;

    public GameObject buildingPrefab;
    private Dictionary<Vector3Int, GameObject> placedBuildings = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, GameObject> GetPlacedBuildings()
    {
        return placedBuildings;
    }

    public bool TryPlacingBuilding(BuildingID id, Vector3Int tilePos) //El metodo principal para colocar los edificios
    {
        BuildingData data = GetBuildingData(id); //Saca los datos del edificio que se quiere construir

        if (!RM.HasResources(data.productionCost))
        {
            Debug.Log("El fallo es aquí");
            return false; //Detecta si el jugador tiene los recursos necesarios
        }
        
        GameObject resourcePrefab = RG.GetResourceAt(tilePos); //Determina que recurso hay
        Debug.Log("Recurso en tile: " + (resourcePrefab ? resourcePrefab.name : "NULL"));
        if (!IsValidPrefab(id, resourcePrefab))
        {
            Debug.Log("El fallo es aquí");
            return false; //Determina si el recurso es el correcto para construir 
        } 
        if (!CheckSpace(tilePos, data.size))
        {
            Debug.Log("El fallo es aquí");
            return false; //Determina si hay espacio
        } 

        RM.SpendResources(data.productionCost); //Gasta los recursos del jugador

        if (resourcePrefab != null) //elimina el recurso que hubiera en la casilla
        {
            Destroy(resourcePrefab);
            RG.RemoveResource(tilePos);
        }

        StartCoroutine(BuildBuilding(tilePos, data)); //Comienza a construirlo

        return true;
    }

    IEnumerator BuildBuilding(Vector3Int pos, BuildingData data)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(pos); //Asegura que el edificio salga en el centro de la casilla
        GameObject construction = Instantiate(buildingPrefab, worldPos, Quaternion.identity); //Instancia el prefab temporal
        float buildingTime = data.productionTime * 60f;
        yield return new WaitForSeconds(buildingTime); //Define el tiempo de construcción en segundos, estando el valor en minutos lo multiplicamos por 60

        Destroy(construction);

        if (data.prefab == null)
        {
            Debug.LogError("Prefab FINAL NULL: " + data.name);
            yield break;
        }

        GameObject finalBuilding = Instantiate(data.prefab, worldPos, Quaternion.identity); //Al terminar la espera coloca el edificio
        placedBuildings[pos] = finalBuilding;

        InitializeBuilding(finalBuilding, data); //Inicializa la logica del edificio
        RegisterBuildingTiles(pos, data.size, finalBuilding); //Almacena la casilla que ocupa el edificio
        yield return null;
    }

    public void InitializeBuilding(GameObject buildingPrefab, BuildingData data) //El metodo para inicializar los datos de logica de los edificios
    {
        BuildingsLogic logic = buildingPrefab.GetComponent<BuildingsLogic>();

        logic.Initialize(data);

        if (logic is FactoriesLogic factory && data is FactoriesData fData)
        {
            factory.Initialize(fData);
        }
        else if (logic is StorageLogic storage && data is StorageData sData)
        {
            storage.Initialize(sData);
        }
        else if (logic is AltarLogic altar && data is AltarData aData)
        {
            altar.Initialize(aData);
        }
    }

    void RegisterBuildingTiles(Vector3Int origin, Vector2Int size, GameObject building)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new Vector3Int(origin.x + x, origin.y + y, 0);
                placedBuildings[pos] = building;
            }
        }
    }

    public BuildingData GetBuildingData(BuildingID id)
    {
        foreach (var b in buildingData)
        {
            if (b == null)
            {
                Debug.LogError("BuildingData NULL en la lista");
                continue;
            }

            if (b.id == id)
            {
                Debug.Log("Encontrado: " + id);
                return b;
            }
        }

        Debug.LogError("NO existe BuildingData para: " + id);
        return null;
    }

    bool IsValidPrefab(BuildingID id, GameObject resourceInstance)
    {
        foreach (var req in buildingRequeriments)
        {
            if (req.id != id) continue;

            if (req.requiredResourceType == Resources.None)
                return resourceInstance == null;

            if (resourceInstance == null)
            {
                Debug.Log("No hay recurso pero se requiere: " + req.requiredResourceType);
                return false;
            }
                
            FountainResourceLogic logic = resourceInstance.GetComponent<FountainResourceLogic>();

            if (logic == null)
            {
                Debug.LogWarning($"El recurso en {resourceInstance.name} no tiene FountainResourceLogic");
                return false;
            }

            return logic.resourceType == req.requiredResourceType;
        }

        return false;
    }

    bool CheckSpace(Vector3Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new Vector3Int(origin.x + x, origin.y + y, 0);

                if (placedBuildings.ContainsKey(pos))
                {
                    return false;
                }
            }
        }
        return true;
    }
}