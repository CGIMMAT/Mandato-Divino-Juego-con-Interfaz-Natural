using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryLogic
{
    public int maxSlots; //El maximo de items almacenables
    public List<InventorySlots> slots;

    public InventoryLogic(int slotsCount) //Añade nuevos slots de inventario si es posible y si hay objetos para esos slots
    {
        maxSlots = slotsCount;
        slots = new List<InventorySlots>();

        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlots(null, 0));
        }
    }

    public bool AddItem(ItemData item, int amount = 1) //Sistema para añadir items a los slots
    {
        if (item.stackable) //Si se puede stackear, varias copias se almacenan en uno
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (slots[i].item == item && slots[i].quantity < item.maxStack)
                {
                    int space = item.maxStack - slots[i].quantity;
                    int toAdd = Mathf.Min(space, amount);

                    slots[i].quantity += toAdd;
                    amount -= toAdd;

                    if (amount <= 0)
                    return true;
                }
            }
        }

        for (int i = 0; i < maxSlots; i++)
        {
            if (slots[i].IsEmpty())
            {
                int toAdd = item.stackable ? Mathf.Min(item.maxStack, amount) : 1;

                slots[i].item = item;
                slots[i].quantity = toAdd;

                amount -= toAdd;

                if (amount <= 0)
                    return true;
            }
        }
        return false;
    }

    public void RemoveItem(int index, int amount = 1) //La logica para quitar items
    {
        if (index < 0 || index >= maxSlots) return;

        InventorySlots slot = slots[index];

        if (slot.IsEmpty()) return;

        slot.quantity -= amount;

        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }
    }
}