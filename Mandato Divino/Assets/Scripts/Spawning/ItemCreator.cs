using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreator : MonoBehaviour //Sistema simplificado del de los edificios para generar items
{
    public ResourceManager RM;

    public Dictionary<ItemData, int> producedItems = new Dictionary<ItemData, int>(); //Sistema para almacenar los objetos generados

    public bool TryProduceItem(ItemData item)
    {
        if (item == null) return false; //Se comprueba que el item exista
        if (!HasResources(item)) return false; //Se comprueba que el jugador tenga los recursos necesarios
        SpendResources(item);
        StartCoroutine(ProduceItem(item));
        return true;
    }

    IEnumerator ProduceItem(ItemData item)
    {
        yield return new WaitForSeconds(item.productionTime * 60);
        AddItem(item);
    }

    void AddItem(ItemData item)
    {
        if (!producedItems.ContainsKey(item))
        {
            producedItems[item] += 0;
        }

        producedItems[item]++;
        Debug.Log("Item producido: " + item.itemName);
    }

    bool HasResources(ItemData item)
    {
        foreach (var cost in item.productionCost)
        {
            int current = GetResource(cost.material);
            if (current < cost.amount) return false;
        }
        return true;
    }
    
        void SpendResources(ItemData item)
    {
        foreach (var cost in item.productionCost)
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
}