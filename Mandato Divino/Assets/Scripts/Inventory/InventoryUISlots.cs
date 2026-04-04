using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUISlots : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;

    public void SetSlots(InventorySlots slot)
    {
        if (slot == null || slot.item == null)
        {
            icon.enabled = false;
            quantityText.text = "";
            return;
        }

        icon.enabled = true;
        // icon.sprite = slot.item.image;

        if (slot.item.stackable && slot.quantity > 1)
        {
            quantityText.text = slot.quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }
}
