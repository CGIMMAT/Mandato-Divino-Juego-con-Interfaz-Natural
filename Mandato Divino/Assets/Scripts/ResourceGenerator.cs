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

    public PrefabsChance[] waterPrefabs; //Prebas para recursos en agua
    public PrefabsChance[] sandPrefabs; //Prefabs para recursos en arena
    public PrefabsChance[] plainPrefabs; //Prefabs para recursos en pradera
    public PrefabsChance[] forestPrefabs; //Prefabs para recusos en vegetación
    public PrefabsChance[] mountainPrefabs; //Prefabs para recursos en montaña

    //En este caso npc son las siglas de "No Prefab Chance"
    public float npcWater = 1.0f; //Probabilidad de que no aparezcan recursos en agua
    public float npcSand = 0.8f; // Probabilidad de que no aparezcan recursos en arena
    public float npcPlain = 0.4f; // Probabilidad de que no aparezcan recursos en pradera
    public float npcForest = 0.3f; // Probabilidad de que no aparezcan recursos en vegetación
    public float npcMountain = 0.5f; // Probabilidad de que no aparezcan recursos en montaña

    public void AssignResources()
    {
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
                    Instantiate(option.prefabs, worldPos, Quaternion.identity);
                    break;
                }
            }
        }
    }
}