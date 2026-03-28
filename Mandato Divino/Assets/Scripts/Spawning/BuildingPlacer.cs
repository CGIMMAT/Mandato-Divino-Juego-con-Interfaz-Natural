using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BuildingRequirements //Sistema para controlar donde se consruyen los edificios
{
    public BuildingID id; //Identifica el tipo de edificio
    public GameObject requiredResource; //Indicaque prefab debe haber para poder construir el edificio
}

public class BuildingPlacer : MonoBehaviour //Código que gestiona la aparición de edificios en el juego intercambiando recursos por estos
{
    public Tilemap tilemap;
    public ResourceManager RM;
    public ResourceGenerator RG;

    public List<BuildingData> buildingData;
    public List<BuildingRequirements> buildingRequeriments;

    public GameObject buildingPrefab;
    private Dictionary<Vector3Int, GameObject> placedBuildings = new Dictionary<Vector3Int, GameObject>();

    public bool TryPlacingBuilding(BuildingID id, Vector3Int tilePos) //El metodo principal para colocar los edificios
    {
        BuildingData data = GetBuildingData(id); //Saca los datos del edificio que se quiere construir

        if (!HasResources(data)) return false; //Detecta si el jugador tiene los recursos necesarios
        GameObject resourcePrefab = RG.GetResourceAt(tilePos); //Determina que recurso hay 
        if (!IsValidPrefab(id, resourcePrefab)) return false; //Determina si el recurso es el correcto para construir 
        if (!CheckSpace(tilePos, data.size)) return false; //Determina si hay espacio

        SpendResources(data); //Gasta los recursos del jugador

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
        yield return new WaitForSeconds(data.productionTime * 60); //Define el tiempo de construcción en segundos, estando el valor en minutos lo multiplicamos por 60

        Destroy(construction);

        GameObject finalBuilding = Instantiate(data.prefab, worldPos, Quaternion.identity); //Al terminar la espera coloca el edificio
        placedBuildings[pos] = finalBuilding;
    }

    BuildingData GetBuildingData(BuildingID id)
    {
        return buildingData.Find(b => b.id == id);
    }

    bool HasResources(BuildingData data)
    {
        foreach (var cost in data.productionCost)
        {
            int current = GetResource(cost.material);
            if (current < cost.amount) return false;
        }
        return true;
    }

    void SpendResources(BuildingData data)
    {
        foreach (var cost in data.productionCost)
        {
            ModifyResource(cost.material, -cost.amount);
        }
    }

    int GetResource(Resources r)
    {
        switch (r)
        {
            case Resources.Food: return RM.food;
            case Resources.Water: return RM.water;
            case Resources.Wood: return RM.wood;
            case Resources.Stone: return RM.stone;
            case Resources.Steel: return RM.steel;
            case Resources.Seeds: return RM.seeds;
        }
        return 0;
    }

    void ModifyResource(Resources r, int amount)
    {
        switch (r)
        {
            case Resources.Food: RM.food += amount; break;
            case Resources.Water: RM.water += amount; break;
            case Resources.Wood: RM.wood += amount; break;
            case Resources.Stone: RM.stone += amount; break;
            case Resources.Seeds: RM.seeds += amount; break;
            case Resources.Steel: RM.steel += amount; break;
        }
    }

    bool IsValidPrefab(BuildingID id, GameObject prefab)
    {
        foreach (var req in buildingRequeriments)
        {
            if (req.id == id)
            {
                if (req.requiredResource == null)
                {
                    return prefab == null;
                }

                if (prefab == null) return false;

                return prefab == req.requiredResource;
            }
        }
        return true;
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