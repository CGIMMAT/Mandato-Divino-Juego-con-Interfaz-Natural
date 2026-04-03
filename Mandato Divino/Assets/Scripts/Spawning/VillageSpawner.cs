using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VillageSpawner : MonoBehaviour //Codigo para controlar los inicios de las partidas
{
    public Tilemap tilemap;
    public ResourceGenerator RG; //Usaremos este código para contolar que zonas pueden ser iniciales por los recursos que tengan
    public ResourceManager RM; //Usamos este código para afectar a los contadores de aldeanos

    public GameObject altarPrefab; //El altar, que será centro de la aldea
    public VillagerSpawner VIS; //El sistema para fabricar aldeanos

    public Vector3Int startPos; //Almacen de la posición iniciañ del altar
    public int maxTrys = 500; //Maximos intentos de encontrar una posición adecuada

    public void spawnAltar() //La función busca la posición adecuada para el altar, a una casilla de agua y en llanura y lo crea si lo encuentra
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int i = 0; i < maxTrys; i++)
        {
            Vector3Int pos = new Vector3Int(Random.Range(bounds.xMin,bounds.xMax), Random.Range(bounds.yMin,bounds.yMax), 0);

            if (!validPlace(pos)) continue;

            GameObject existing = RG.GetResourceAt(pos); //Comprueba si hay recursos donde debe aparecer el atar para destruirlos
            if (existing != null)
            {
                Destroy(existing);
                RG.RemoveResource(pos);
            }

            Vector3 worldPos = tilemap.GetCellCenterWorld(pos); //Asigna el altar a la casilla
            Instantiate(altarPrefab, worldPos, Quaternion.identity);
            startPos = pos;

            spawnVillagers(pos); //Genera los aldeanos alrededor del altar
            return;
        }
    }

    public bool validPlace(Vector3Int pos)
    {
        TileBase tileBase = tilemap.GetTile(pos); //Coge la posición del tile
        PrefabTile tile = tileBase as PrefabTile; //Coge el tile en sí

        if (tile.level != 2) return false; //Solo deja escoger casillas de llanuras
        if (!nearWater(pos)) return false; //Solo deja escoger casillas que estén cerca del agua dulce
        return true; //Si se cumplen ambas entonces se genera correctamente
    }

    public bool nearWater(Vector3Int pos)
    {
        Vector3Int[] directions = new Vector3Int[] //Almacenamos todas las posibles direcciones respecto al altar donde podría coloarse el recurso de agua
        {
            new Vector3Int(2,0,0),
            new Vector3Int(-2,0,0),
            new Vector3Int(0,2,0),
            new Vector3Int(0,-2,0),
            new Vector3Int(2,2,0),
            new Vector3Int(-2,2,0),
            new Vector3Int(2,-2,0),
            new Vector3Int(-2,-2,0),
        };

        foreach (var dir in directions)
        {
            Vector3Int checkPos = pos + dir;
            GameObject res = RG.GetResourceAt(checkPos);

            if (res != null && res.CompareTag("Water")) return true;
        }
        return false;
    }

    public void spawnVillagers(Vector3Int pos)//El sistema hara aparecer 8 aldeanos, uno en cada casilla adyacente al altar
    {
        for (int y = -1; y <=1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue; //Se salta la casilla del medio

                Vector3Int villagersPos = new Vector3Int(pos.x + x, pos.y + y, 0);
                Vector3 worldPos = tilemap.GetCellCenterWorld(villagersPos);
                VIS.GenerateInitial(worldPos); //Genera un aldeano inicial en la casilla
                RM.population++;
            }
        }
    }
}