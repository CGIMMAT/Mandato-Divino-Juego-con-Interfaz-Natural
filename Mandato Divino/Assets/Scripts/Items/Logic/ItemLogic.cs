using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogic : MonoBehaviour //La inicialización de los datos genericos de items en todos estos
{
    public string itemName;
    public ItemID itemID;
    public ActionType actionType;

    public bool isStackable;
    public int maxStack;

    public bool isEquipable;
    public bool isEquiped;

    protected ItemData data;

    public virtual void Initialize(ItemData itemData)
    {
        data = itemData;

        itemName = data.itemName;
        itemID = data.id;
        actionType = data.actionType;

        isStackable = data.isStackable;
        maxStack = data.maxStack;

        isEquipable = data.isEquipable;
        isEquiped = false;
    }
}