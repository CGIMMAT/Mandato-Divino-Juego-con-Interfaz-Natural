using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionBase : MonoBehaviour //Base de funcionamiento para casi todas las fábricas, solamente no se empleará en el Herrero
{
    protected FactoriesLogic factory; //Logica de funcionamiento de ls fabricas
    protected ResourceManager RM; //Recursos que generan

    protected virtual void Start() //Se inicializan ambos valores
    {
        factory = GetComponent<FactoriesLogic>();
        RM = FindObjectOfType<ResourceManager>();
    }

    protected void DistributeItem(BaseItemsData item, int amount) //Sistema para distribuir los items producidos
    {
        if (item == null || amount <= 0) return;

        int remaining = amount; //La cantidad de items restantes
        remaining = SendToStorages(item, remaining); //Se envían primero a almacenes
        remaining = SendToWorkers(item, remaining); //Luego a los trabajadores de la fabrica
        if (remaining > 0) factory.inventory.AddItem(item, remaining); //Por último al almacen de la fabrica
    }

    private int SendToStorages(BaseItemsData item, int amount)
    {
        foreach (var storage in FindObjectsOfType<StorageLogic>())
        {
            if (!storage.CanStore(item)) continue; //Si el almacen no puede almacenar este tipo de recurso, se ignora

            int sent = storage.inventory.AddItem(item, amount);
            amount -= sent;

            if (amount <= 0) return 0;
        }

        return amount;
    }

    private int SendToWorkers(BaseItemsData item, int amount)
    {
        foreach (var worker in factory.assignedWorkers)
        {
            if (worker == null) continue;

            int sent = worker.inventory.AddItem(item, amount);
            amount -= sent;

            if (amount <= 0) return 0;
        }

        return amount;
    }
}