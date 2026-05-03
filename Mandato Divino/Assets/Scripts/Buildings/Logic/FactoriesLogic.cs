using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoriesLogic : BuildingsLogic //La inicialización de los datos específicos de las fábricas
{
    public Resources producedMaterial;
    public float productionRate;
    public int maxStorage;

    public int currentStorage;

    public int workersAssigned;
    public int maxWorkers;
    public List<VillagerLogic> assignedWorkers = new List<VillagerLogic>();

    public InventoryLogic inventory;

    public bool needsWorker;
    public bool HasFreeWorkerSpace()
    {
        return workersAssigned < maxWorkers;
    }

    public void Initialize(FactoriesData data)
    {
        base.Initialize(data);

        producedMaterial = data.producedMaterial;
        productionRate = data.productionRate;
        maxStorage = data.maxStorage;

        maxWorkers = data.workBenches;
        needsWorker = data.needsWorker;

        currentStorage = 0;
        inventory = new InventoryLogic(maxStorage);
    }
}