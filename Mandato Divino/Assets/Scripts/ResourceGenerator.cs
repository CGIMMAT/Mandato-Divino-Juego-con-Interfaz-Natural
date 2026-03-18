using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable] //Sistema para almacenar los distintos prefabs que pueden aparecer para cada tipo de tile y su probabilidadde aparición
public class PrefabsChance
{
    public GameObject prefabs;
    public float chance;
}

public class ResourceGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    
    //Sección para recursos permanentes
    public PrefabsChance[] waterPrefabs; //Prefabs para recursos en agua
    public PrefabsChance[] sandPrefabs; //Prefabs para recursos en arena
    public PrefabsChance[] plainPrefabs; //Prefabs para recursos en pradera
    public PrefabsChance[] forestPrefabs; //Prefabs para recusos en vegetación
    public PrefabsChance[] mountainPrefabs; //Prefabs para recursos en montaña

    //En este caso npc son las siglas de "No Prefab Chance"
    public float npcWater = 1.0f; //Probabilidad de que no aparezcan recursos en agua
    public float npcSand = 0.98f; // Probabilidad de que no aparezcan recursos en arena
    public float npcPlain = 0.95f; // Probabilidad de que no aparezcan recursos en pradera
    public float npcForest = 0.9f; // Probabilidad de que no aparezcan recursos en vegetación
    public float npcMountain = 0.8f; // Probabilidad de que no aparezcan recursos en montaña

    private Dictionary<Vector3Int, GameObject> spawnedResources = new Dictionary<Vector3Int, GameObject>(); //Se usa para almacenar las casillas en específico donde hay un recurso permanente

    //Sección para recursos temporales
    public PrefabsChance[] temporaryWater; //Recursos temporales en agua
    public PrefabsChance[] temporarySand; //Recursos temporales en arena
    public PrefabsChance[] temporaryPlain; //recursos temporales en pradera
    public PrefabsChance[] temporaryForest; //Recursos temporales en vegetación
    public PrefabsChance[] temporaryMountain; //Recursos temporales en montaña

    public float npcTemporaryWater = 1f; //Probabilidad de que no aparezcan recursos temporales en agua
    public float npcTemporarySand = 0.99f; //Probabilidad de que no aparezcan recursos temporales en arena
    public float npcTemporaryPlain = 0.97f; //Probabilidad de que no aparezcan recursos temporales en pradera
    public float npcTemporaryForest = 0.95f; //Probabilidad de que no aparezcan recursos temporales en vegetación
    public float npcTemporaryMountain = 0.98f; //Probabilidad de que no aparezcan recursos temporales en montaña

    //Ahora invocamos los tiempos que se usaran para etablecer un intervalo de genración y destrucción de recursos temporales
    public float spwanMin = 3f;
    public float spawnMax = 10f;

    public float destroyMin = 20f;
    public float destroyMax = 50f;

    public int resourceLimit = 50; //Limite de recursos temporales que se generan 

    public void Initialize()
    {
        AssignResources();
        StartCoroutine(spawnResources());
    }

    public void AssignResources()
    {
        spawnedResources.Clear(); //Metodo de seguridad por si al generar el mapa aún hay recursos generados

        BoundsInt bounds = tilemap.cellBounds; //Asegura que se recorran todas las celdas con tiles del grid

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tileBase = tilemap.GetTile(pos);
            PrefabTile tile = tileBase as PrefabTile;

            PrefabsChance[] options = null; // Se inicializan las variables para almacenar prefabs y probabilidades dentro del metodo
            float npc = 0f;

            switch (tile.level)//Para cada posible tile,se asignaran solo unos prefabs en concreto con sus probabilidades asociadas
            {
                case 0: options = waterPrefabs; npc = npcWater; break;
                case 1: options = sandPrefabs; npc = npcSand; break;
                case 2: options = plainPrefabs; npc = npcPlain; break;
                case 3: options = forestPrefabs; npc = npcForest; break;
                case 4: options = mountainPrefabs; npc = npcMountain; break;
            }

            float roll = Random.Range(0f, 1f);
            if (roll < npc) continue; //Si la tirada aleatoria es menor, entoncesla casilla no tendrá ningún recurso

            roll = (roll - npc) / (1f - npc); //Se ajusta el valor antes de decidir el recurso

            float cumulative = 0f;
            foreach (var option in options)
            {
                cumulative += option.chance;
                if (roll < cumulative)
                {
                    Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                    GameObject resource = Instantiate(option.prefabs, worldPos, Quaternion.identity);
                    spawnedResources[pos] = resource;
                    break;
                }
            }
        }
    }

    IEnumerator spawnResources() //Metodo para generar recursos temporales en casillas y momentos aleatorios
    {
        BoundsInt bounds = tilemap.cellBounds; //Se vuelve a invocar todo el tileset

        while (true)
        {
            float waitTime = Random.Range(spwanMin, spawnMax);
            yield return new WaitForSeconds(waitTime);

            int resourceBatch = Random.Range(1, resourceLimit + 1); //La catidad de recursos aleatorios generados

            for (int r = 0; r < resourceBatch; r++)
            {
                Vector3Int pos = new Vector3Int(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax), 0);
                if (spawnedResources.ContainsKey(pos)) continue; //Si la casilla elegida ya tiene un recurso fijo se pasa a la siguiente iteración

                TileBase tileBase = tilemap.GetTile(pos); //Repetimos el funcionamiento de asignación de recursos de antes
                PrefabTile tile = tileBase as PrefabTile;

                PrefabsChance[] options = null;
                float npc = 0f;

                switch (tile.level)
                {
                    case 0: options = temporaryWater; npc = npcTemporaryWater; break;
                    case 1: options = temporarySand; npc = npcTemporarySand; break;
                    case 2: options = temporaryPlain; npc = npcTemporaryPlain; break;
                    case 3: options = temporaryForest; npc = npcTemporaryForest; break;
                    case 4: options = temporaryMountain; npc = npcTemporaryMountain; break;
                }

                float roll = Random.Range(0f, 1f);
                if (roll < npc) continue;

                roll = (roll - npc) / (1f - npc);

                float cumulative = 0;
                foreach (var option in options)
                {
                    cumulative += option.chance;
                    if (roll < cumulative)
                    {
                        Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                        GameObject resource = Instantiate(option.prefabs, worldPos, Quaternion.identity);
                        spawnedResources[pos] = resource;

                        float lifetime = Random.Range(destroyMin, destroyMax);
                        StartCoroutine(destroyResources(pos, resource, lifetime));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator destroyResources(Vector3Int pos, GameObject resource, float lifetime) //Metodo para la destrucción de los recursos temporales
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(resource);
        spawnedResources.Remove(pos);
    }
}