using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissasterManager : MonoBehaviour
{
    public float disasterChance = 5f; //La probabilidad de que ocurra un desastre natural
    public float disasterTimer = 600f; //El tiempo que puede pasar hasta que otro suceda 
    public float disasterIntesity = 1f; //La intensidad de cada desastre, que marcará los daños que causan, estas variables se pueden ver modificas en una escala de 10 niveles

    public GameObject firePrefab; //Los prefabs que se instanciaran con cada desastre
    public GameObject stormPrefab;
    public GameObject quakePrefab;

    public ResourceGenerator RG;
    public MapGenerator MG;
    private Coroutine disasterRoutine;

    private void Start()
    {
        disasterRoutine = StartCoroutine(DisasterLoop());
    }

    IEnumerator DisasterLoop()
    {
        while (true)
        {
            float actualTimer = disasterTimer - (DifficultGenerator.instance.currentLevel - 1) *60f;
            yield return new WaitForSeconds(actualTimer);
            LaunchDisaster();
        }
    }

    public void LaunchDisaster()
    {
        int level = DifficultGenerator.instance.currentLevel;
        float actualChance = disasterChance + (level - 1) * 5;


        if (Random.Range(0f, 100f) < actualChance) //En base a las probabilidades, se ejecutará un desastre natural
        {
            int disaster = Random.Range(0,3); //El tipo también será aleatorio

            switch (disaster)
            {
                case 0: LaunchFire(); break;
                case 1: LaunchStorm(); break;
                case 2: LaunchQuake(); break;
            }
        }
    }

    public void LaunchFire()
    {
        var resources = RG.GetType().GetField("PublicSpawnedResources", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(RG) as Dictionary<Vector3Int, GameObject>;

        if (resources != null && resources.Count > 0)
        {
            List<Vector3Int> keys = new List<Vector3Int>(resources.Keys);
            Vector3Int randomPos = keys[Random.Range(0, keys.Count)];
            Instantiate(firePrefab, RG.tilemap.GetCellCenterWorld(randomPos), Quaternion.identity).GetComponent<FireLogic>().Initialize(randomPos, disasterIntesity, RG);
        }
    }

    public void LaunchStorm()
    {
        Vector3Int randomTile = GetRandomTile();
        Instantiate(stormPrefab, RG.tilemap.GetCellCenterWorld(randomTile), Quaternion.identity).GetComponent<StormLogic>().Initialize(disasterIntesity);
    }

    public void LaunchQuake()
    {
        Vector3Int randomTile = GetRandomTile();
        Instantiate(quakePrefab, RG.tilemap.GetCellCenterWorld(randomTile), Quaternion.identity).GetComponent<QuakeLogic>().Initialize(disasterIntesity);
    }

    Vector3Int GetRandomTile()
    {
        int x = Random.Range(0, MG.SizeXY);
        int y = Random.Range(0, MG.SizeXY);
        return new Vector3Int(x, y, 0);
    }
}
