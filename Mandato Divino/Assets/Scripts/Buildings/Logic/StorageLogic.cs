using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageLogic : BuildingsLogic //La inicialización de datos específicos e almacenes
{
    public Resources storedMaterial;
    public StorageData data;
    public InventoryLogic inventory;

    public void Initialize(StorageData storageData)
    {
        base.Initialize(storageData);
        data = storageData;
        inventory = new InventoryLogic(data.storageCapacity);
    }

    public bool HasFreeSpace()
    {
        foreach (var slot in inventory.slots)
        {
            if (slot.IsEmpty()) return true;

            if (slot.item != null && slot.item.resourceType == data.storedMaterial && slot.quantity < slot.item.maxStack) return true;
        }
        return false;
    }

    public bool CanStore(BaseItemsData item)
    {
        return item.resourceType == data.storedMaterial;
    }
}