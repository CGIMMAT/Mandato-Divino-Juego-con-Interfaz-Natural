using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawner : MonoBehaviour //Codigo qu gestiona la aparición de monstruos en el mapa
{
    public Tilemap tilemap;
    public ResourceGenerator RG; //Se usará para poder saber que casillas tienen recursos y edificios

    public GameObject monsterPrefab; //Los monstruos prefabs

    public int maxMonsters = 5; //La cantidad máxima de mosntruos que puede haber al mismo tiempo
    public float minSpawnTime = 60f; //Los tiempos de carga entre que aparezcan nuevos monstruos
    public float maxSpawnTime = 360f;

    public List<GameObject> allMonsters = new List<GameObject>(); //Los monstruos activos

    public void Initialize()
    {
        StartCoroutine(spawnMonsters());
    }

    IEnumerator spawnMonsters()
    {
        BoundsInt bounds = tilemap.cellBounds;

        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime); //Se espera un tiempo semialeatorio y modificable entre cada aparición
            yield return new WaitForSeconds(waitTime);
            allMonsters.RemoveAll(item => item == null); //Y por si acaso se eliminan el registro de monstruos activos los monstruos por si se ha matado alguno

            if (allMonsters.Count < maxMonsters) //Si no hay demasiados monstruos,se intenta genrar uno
            {
                TrySpawnMonster(bounds);
            }
        }
    }

    public void TrySpawnMonster(BoundsInt bounds) //El metodo para intentar generar monstruos
    {
        for (int i = 0; i < 100; i++) //No se hacen intentos infinitos, se establece un máximo de intentos
        {
            Vector3Int randomPos = new Vector3Int(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax), 0);
            if (ValidMonsterPosition(randomPos))
            {
                Vector3 worldPos = tilemap.GetCellCenterWorld(randomPos);
                GameObject monster = Instantiate(monsterPrefab, worldPos, Quaternion.identity);
                MonsterLogic logic = monster.GetComponent<MonsterLogic>();
                logic.ApplyDifficultyBonus(DifficultGenerator.instance.currentLevel);

                allMonsters.Add(monster); //Después de inicializar los datos del monstruo, se añden a la casilla y la lista de monstruos activos
                break;
            }
        }
    }

    public bool ValidMonsterPosition(Vector3Int pos) //El sistema de comprobación de que la casilla donde se spawnea es valida
    {
        TileBase tileBase = tilemap.GetTile(pos);
        if (tileBase == null) return false; //Primero, se comprueba que la casilla tenga un tile cargado

        PrefabTile tile = tileBase as PrefabTile;
        if (tile == null && tile.level == 0) return false; //Luiego que la casilla es de un tile que no sea agua, que este en tierra

        if (RG.GetResourceAt(pos) != null) return false; //Por ultimo, se comprueba que la casilla no tiene nada, ni recursos ni edificios
        return true;
    }

    public void UpdateDifficulty(int level)
    {
        maxMonsters = 5 + (level - 1) * 2;

        minSpawnTime = 60f - (level - 1) * 5f;
        maxSpawnTime = 360f - (level - 1) * 30f;
    }

    public void RemoveMonster(GameObject monster)
    {
        if (allMonsters.Contains(monster)) allMonsters.Remove(monster);
    }
}
