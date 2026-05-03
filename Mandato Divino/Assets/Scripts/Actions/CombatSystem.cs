using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatSystem //La lógica que definirá nuestroc ombate en el juego
{
    public static void Attack(VillagerLogic attacker, CombatTarget target) //Primero definimos los ataques de los aldeanos
    {
        if (attacker == null || target == null) return; //Para ello se necesita ue haya un aldeano y un enemigo

        int damage = attacker.attackDamage; //El daño base ya estaba definido en VillagerLogicy Data

        ItemInstance item = attacker.equipedItem; //También tendremos en cuenta si el aleano cuenta con un arma
        ActionType attackerType = ActionType.None;
        ActionType defenderType = ActionType.None;

        if (item != null && item.data is WeaponData weapon)
        {
            damage = weapon.damage; //Si si tiene un arma su daño pasa a ser el daño de ataque basico
            attackerType = weapon.actionType;

            if (weapon.actionType == ActionType.Disparar)
            {
                if (!ConsumeProjectile(attacker.inventory, ActionType.Disparar)) return;
            }

            if (weapon.actionType == ActionType.Proyectil)
            {
                if (!ConsumeProjectile(attacker.inventory, ActionType.Proyectil)) return;
            }

            if (item.HasDurability())
            {
                item.Use();
            }       
        }

        if (target is VillagerLogic defender && defender.equipedItem != null)
        {
            if (defender.equipedItem.data is WeaponData defenderWeapon)
            {
                defenderType = defenderWeapon.actionType;
                damage = ApplyWeaponAdvantage(damage, attackerType, defenderType);
            }
        }

        target.TakeDamage(damage); //El enemigo recibe daño
    }

    static int ApplyWeaponAdvantage(int damage, ActionType attacker, ActionType defender)
    {
        if (attacker == ActionType.None || defender == ActionType.None)
            return damage;

        bool advantage =
            (attacker == ActionType.Rajar && defender == ActionType.Clavar) ||
            (attacker == ActionType.Clavar && defender == ActionType.Aplastar) ||
            (attacker == ActionType.Aplastar && defender == ActionType.Rajar);

        bool disadvantage =
            (defender == ActionType.Rajar && attacker == ActionType.Clavar) ||
            (defender == ActionType.Clavar && attacker == ActionType.Aplastar) ||
            (defender == ActionType.Aplastar && attacker == ActionType.Rajar);

        if (advantage)
        {
            return Mathf.CeilToInt(damage * 1.25f);
        }

        if (disadvantage)
        {
            return Mathf.CeilToInt(damage * 0.75f);
        }

        return damage;
    }

    static bool ConsumeProjectile(InventoryLogic inventory, ActionType type)
    {
        foreach (var slot in inventory.slots)
        {
            if (slot.IsEmpty()) continue;
            if (slot.item is not ItemData data) continue;

            if (data.actionType == type)
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
