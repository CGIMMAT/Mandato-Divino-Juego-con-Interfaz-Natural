using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreator : MonoBehaviour //Sistema simplificado del de los edificios para generar items
{
    public ResourceManager RM;
    public BuildingPlacer BP; //Logica para encontrar las herrerías

    public Dictionary<ItemData, int> producedItems = new Dictionary<ItemData, int>(); //Sistema para almacenar los objetos generados

    public bool TryProduceItem(ItemData item)
    {
        if (item == null) return false; //Se comprueba que el item exista
        if (!RM.HasResources(item.productionCost)) return false; //Se comprueba que el jugador tenga los recursos necesarios
        RM.SpendResources(item.productionCost);
        StartCoroutine(ProduceItem(item));
        return true;
    }

    IEnumerator ProduceItem(ItemData item) //Corrutina para dejar creando el item al jugador durante su tiempo de produccion
    {
        yield return new WaitForSeconds(item.productionTime * 60);
        AddItem(item);
    }

    void AddItem(ItemData item) //Se añade el item al inventario del que lo ha creado
    {
        if (!producedItems.ContainsKey(item))
        {
            producedItems[item] = 0;
        }

        producedItems[item]++;
    }

    public FactoriesLogic GetClosestBlacksmith(Vector3 pos) //Codigo para encontar la herreriamás cercana
    {
        FactoriesLogic closest = null;
        float bestDist = Mathf.Infinity;

        var buildings = BP.GetPlacedBuildings(); //Se acceden a los edificios ya colocados

        foreach (var b in buildings)
        {
            GameObject obj = b.Value;
            if (obj == null) continue;

            FactoriesLogic factory = obj.GetComponent<FactoriesLogic>();
            if (factory == null) continue;

            if (factory.buildingID != BuildingID.Herreria) continue; //Se revisa la id de estos hasta encontrar coicidencias

            float d = Vector3.Distance(pos, obj.transform.position);

            if (d < bestDist)
            {
                bestDist = d;
                closest = factory;
            }
        }

        return closest; //Se da la posición más cercana
    }
}