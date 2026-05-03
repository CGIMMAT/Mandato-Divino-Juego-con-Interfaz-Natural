using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUISlots : MonoBehaviour //El encargado de gestionar los slots del inventario visualmente
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    public void SetData(InventorySlots slot)
    {
        if (slot == null || slot.IsEmpty())
        {
            icon.enabled = false;
            quantityText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = slot.item.icon;

        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
    }
}