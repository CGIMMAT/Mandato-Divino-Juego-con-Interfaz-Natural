using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerSpawner : MonoBehaviour //Sistema para crear aldeanos, atnto los iniciales como los nacidos
{
    public VillagersData villagerData; //Los datos genericos de los aldeanos
    private static int idCounter = 0; //El contador del id, este actuara como un int y cada nuevo aldeano recibe un valor de +1
    public List<VillagerLogic> allVillagers = new List<VillagerLogic>(); //Lista donde se almacenan los datos de todos los aldeanos

    public WorldResourceData foodItem; //Items iniciales para los aldeanos, comida,
    public WorldResourceData woodItem; //Madera,
    public WorldResourceData stoneItem; //Y piedra

    public void InitialResources(VillagerLogic villager)//Función para dar a los aldeanos inciales unos recursos básicos
    {
        villager.inventory.AddItem(foodItem, foodItem.maxStack); //Se les da un stack de comida, uno de madera y uno de piedra
        villager.inventory.AddItem(woodItem, woodItem.maxStack);
        villager.inventory.AddItem(stoneItem, stoneItem.maxStack);
    }

    public VillagerLogic GenerateInitial(Vector3 pos) //La función para los primeros aldeanos, que siempre serán adultos
    {
        VillagerLogic v = GenerateVillager(pos, Age.Adulto);
        InitialResources(v); //Se les da los recursos iniciales
        return v;
    }

    public VillagerLogic GenerateChild(Vector3 pos) //La función para los demás, que siempre serán niños
    {
        VillagerLogic v = GenerateVillager(pos, Age.Niño);
        return v;
    }

    public VillagerLogic GenerateVillager(Vector3 pos, Age age) //La funciñon que genera en una posición a los aldeanos
    {
        GameObject villagerGO = Instantiate(villagerData.prefab, pos, Quaternion.identity);
        VillagerLogic villager = villagerGO.GetComponent<VillagerLogic>();
        villager.Initialize(villagerData, idCounter++, age);
        allVillagers.Add(villager); //Se añade el aldeano recien creado a la lista
        return villager;
    }

    public void RemoveVillager(VillagerLogic villager)
    {
        if (allVillagers.Contains(villager)) allVillagers.Remove(villager); //Metodo para eliminar de la lista de aldeanos al aldeano que ha muerto
    }
}