using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLogic : ItemLogic //Se inicializan los datos propios de las herramientas
{
    public int durability;
    public int effectivity;

    public override void Initialize(ItemData itemData)
    {
        base.Initialize(itemData);

        if (itemData is ToolData tool)
        {
            durability = tool.durability;
            effectivity = tool.effectivity;
        }
        else
        {
            Debug.LogError("ItemData no es ToolData");
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
}