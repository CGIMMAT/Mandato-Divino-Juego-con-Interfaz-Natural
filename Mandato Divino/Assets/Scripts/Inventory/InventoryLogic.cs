using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryLogic
{
    public int maxSlots;
    public List<InventorySlots> slots;

    public InventoryLogic(int slotsCount)
    {
        maxSlots = slotsCount;
        slots = new List<InventorySlots>();

        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlots());
        }
    }

     public int AddItem(BaseItemsData item, int amount = 1)
    {
        UpdateSpecialSlots();

        int initialAmount = amount;

        ItemData data = item as ItemData;

        if (!item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.IsEmpty() && IsValidSlot(slot, item))
                {
                    slot.item = item;
                    slot.instance = data != null ? new ItemInstance(data) : null;
                    slot.quantity = 1;

                    amount--;
                    if (amount <= 0) return initialAmount;
                }
            }
            return initialAmount - amount;
        }

        foreach (var slot in slots)
        {
            if (slot.item == item && slot.quantity < item.maxStack)
            {
                if (!IsValidSlot(slot, item)) continue;

                if (slot.allowedResourceType == Resources.Agua)
                {
                    if (slot.quantity >= 1) continue;

                    slot.quantity = 1;
                    amount--;
                }
                else
                {
                    int space = item.maxStack - slot.quantity;
                    int toAdd = Mathf.Min(space, amount);

                    slot.quantity += toAdd;
                    amount -= toAdd;
                }

                if (amount <= 0) return initialAmount;
            }
        }

        foreach (var slot in slots)
        {
            if (slot.IsEmpty() && IsValidSlot(slot, item))
            {
                if (slot.allowedResourceType == Resources.Agua)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    slot.instance = null;

                    amount--;
                }
                else
                {
                    int toAdd = Mathf.Min(item.maxStack, amount);

                    slot.item = item;
                    slot.quantity = toAdd;
                    slot.instance = null;

                    amount -= toAdd;
                }

                if (amount <= 0) return initialAmount;
            }
        }

        return initialAmount - amount;
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= maxSlots) return;

        slots[index] = new InventorySlots();
    }

    public void UpdateSpecialSlots()
    {
        bool hasBow = false;
        bool hasBallista = false;
        bool hasBucket = false;

        int backpackSlots = 0;

        foreach (var slot in slots)
        {
            if (slot.IsEmpty()) continue;

            if (slot.instance != null && slot.instance.data is BackpackData backpack)
            {
                backpackSlots += backpack.slots;
            }

            if (slot.instance != null)
            {
                var id = slot.instance.data.id;

                if (id == ItemID.Arco) hasBow = true;
                if (id == ItemID.Balista) hasBallista = true;

                if (id == ItemID.Cubo) hasBucket = true;
            }
        }

        int normalSlots = 5 + backpackSlots;
        int arrowSlots = hasBow ? 30 : 0;
        int projectileSlots = hasBallista ? 30 : 0;
        int waterSlots = hasBucket ? backpackSlots : 0;

        int totalSlots = normalSlots + arrowSlots + projectileSlots + waterSlots;

        if (totalSlots == maxSlots) return;

        List<InventorySlots> newSlots = new List<InventorySlots>();

        for (int i = 0; i < normalSlots; i++)
        {
            newSlots.Add(new InventorySlots
            {
                slotType = SlotType.Normal
            });
        }

        for (int i = 0; i < arrowSlots; i++)
        {
            newSlots.Add(new InventorySlots
            {
                slotType = SlotType.Flecha
            });
        }

        for (int i = 0; i < projectileSlots; i++)
        {
            newSlots.Add(new InventorySlots
            {
                slotType = SlotType.Proyectil
            });
        }

        for (int i = 0; i < waterSlots; i++)
        {
            newSlots.Add(new InventorySlots
            {
                slotType = SlotType.Normal,
                allowedResourceType = Resources.Agua
            });
        }

        foreach (var old in slots)
        {
            if (old.IsEmpty()) continue;

            TryInsertIntoNewSlots(newSlots, old.item, old.quantity, old.instance);
        }

        slots = newSlots;
        maxSlots = totalSlots;
    }

    public void TryInsertIntoNewSlots(List<InventorySlots> newSlots, BaseItemsData item, int amount, ItemInstance instance)
    {
        foreach (var slot in newSlots)
        {
            if (!slot.IsEmpty()) continue;

            if (IsValidSlot(slot, item))
            {
                slot.item = item;
                slot.quantity = amount;
                slot.instance = instance;
                return;
            }
        }
    }

    bool IsValidSlot(InventorySlots slot, BaseItemsData item)
    {
        if (item == null) return false;

        if (slot.allowedResourceType == Resources.Agua)
        {
            return item.resourceType == Resources.Agua;
        }

        if (slot.slotType == SlotType.Flecha)
        {
            return item is ItemData d && d.actionType == ActionType.Disparar;
        }

        if (slot.slotType == SlotType.Proyectil)
        {
            return item is ItemData d && d.actionType == ActionType.Proyectil;
        }

        return true;
    }
}