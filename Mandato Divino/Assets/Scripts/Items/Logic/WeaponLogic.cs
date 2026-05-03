using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : ItemLogic //Se inicializan los datos propios de las armas
{
    public int durability;
    public int damage;
    public float attackRange;
    public float attackSpeed;
    public InventoryLogic bulletInventory;

    public override void Initialize(ItemData itemData)
    {
        base.Initialize(itemData);

        if (itemData is WeaponData weapon)
        {
            durability = weapon.durability;
            damage = weapon.damage;
            attackRange = weapon.attackRange;
            attackSpeed = weapon.attackSpeed;

            if (weapon.actionType == ActionType.Lanzar || weapon.actionType == ActionType.Artilleria)
            {
                bulletInventory = new InventoryLogic(30);
            }
        }
        else
        {
            Debug.LogError("ItemData no es WeaponData");
        }
    }

    public virtual void Use()
    {
        durability--;

        if (durability <= 0)
        {
            BreakItem();
        }
    }

    protected virtual void BreakItem()
    {
        Destroy(gameObject);
    }

    public bool TryConsumeProjectile(ActionType neededType)
    {
        if (bulletInventory == null) return false;

        foreach (var slot in bulletInventory.slots)
        {
            if (slot.IsEmpty()) continue;
            if (slot.item is not ItemData data) continue;

            if (data.actionType == neededType)
            {
                slot.quantity--;

                if (slot.quantity <= 0)
                    slot.item = null;

                return true;
            }
        }

        return false;
    }
}