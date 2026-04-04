using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlots //Código para definir la lógica de cada slot que puede tener el inventario de los aldeanos
{
    public ItemData item; //El item que se almacena en el slot
    public int quantity; //Su cantidad, que puede variar al ser stackeable

    public InventorySlots(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public bool IsEmpty() //No hay slot si no hay item
    {
        return item == null;
    }
}
