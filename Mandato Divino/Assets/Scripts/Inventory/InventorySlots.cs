using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    Normal,
    Flecha,
    Proyectil
}

[System.Serializable]
public class InventorySlots //Código para definir la lógica de cada slot que puede tener el inventario de los aldeanos
{
    public BaseItemsData item;
    public int quantity;

    public ItemInstance instance;
    public SlotType slotType = SlotType.Normal;
    public ActionType allowedActionType = ActionType.None;
    public Resources allowedResourceType = Resources.None;

    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }

    public bool IsStackable()
    {
        return item != null && item.isStackable;
    }

    public ItemData GetItemData()
    {
        if (instance != null) return instance.data;
        return item as ItemData;
    }
}