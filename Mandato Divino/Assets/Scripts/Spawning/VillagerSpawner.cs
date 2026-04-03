using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerSpawner : MonoBehaviour //Sistema para crear aldeanos, atnto los iniciales como los nacidos
{
    public VillagersData villagerData; //Los datos genericos de los aldeanos
    private static int idCounter = 0; //El contador del id, este actuara como un int y cada nuevo aldeano recibe un valor de +1

    public VillagerLogic GenerateInitial(Vector3 pos)
    {
        return GenerateVillager(pos, Age.Adult);
    }

    public VillagerLogic GenerateChild(Vector3 pos)
    {
        return GenerateVillager(pos, Age.Child);
    }

    public VillagerLogic GenerateVillager(Vector3 pos, Age age)
    {
        GameObject villagerGO = Instantiate(villagerData.prefab, pos, Quaternion.identity);
        VillagerLogic villager = villagerGO.GetComponent<VillagerLogic>();
        villager.Initialize(villagerData, idCounter++, age);
        return villager;
    }
}