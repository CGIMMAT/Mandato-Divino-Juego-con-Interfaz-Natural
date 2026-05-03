using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackLogic : ItemLogic //La inicialización de los datos en el prefab
{
    public int slots;

    public override void Initialize(ItemData itemData)
    {
        base.Initialize(itemData);

        if (itemData is BackpackData backpack)
        {
            slots = backpack.slots;
        }
        else
        {
            Debug.LogError("ItemData no es BackpackData");
        }
    }
}
