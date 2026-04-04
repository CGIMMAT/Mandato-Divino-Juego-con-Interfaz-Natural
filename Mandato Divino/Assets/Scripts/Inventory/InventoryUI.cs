using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform gridParent;

    private VillagerLogic currentVillager;
    private List<InventoryUISlots> slots = new List<InventoryUISlots>();

    public void OpenInventory(VillagerLogic villager)
    {
        gameObject.SetActive(true);
        currentVillager = villager;

        RefreshUI();
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
        currentVillager = null;
    }

    void RefreshUI()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        slots.Clear();

        for (int i = 0; i < currentVillager.inventory.maxSlots; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, gridParent);
            InventoryUISlots slotUI = slotGO.GetComponent<InventoryUISlots>();

            slotUI.SetSlots(currentVillager.inventory.slots[i]);

            slots.Add(slotUI);
        }
    }
}
