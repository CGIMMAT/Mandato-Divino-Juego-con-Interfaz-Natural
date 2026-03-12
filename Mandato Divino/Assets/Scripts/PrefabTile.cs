using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/PrefabTile")]
public class PrefabTile : Tile
{
    public GameObject prefab; //Los posibles prefabs asignados a una casilla del mapa
    public int level; //Los niveles asociados a cada tipo de tile
}
