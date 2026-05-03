using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagersUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform gridParent;

    public VillagerInfoUI infoPanel;

    public VillagerSpawner villagerManager;

    private List<VillagerSlotUI> slots = new List<VillagerSlotUI>();

    public void OpenUI()
    {
        gameObject.SetActive(true);
        GenerateList();
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
        infoPanel.Hide();
    }

    void GenerateList()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        slots.Clear();

        foreach (VillagerLogic v in villagerManager.allVillagers)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            VillagerSlotUI slot = go.GetComponent<VillagerSlotUI>();

            slot.SetData(v, this);
            slots.Add(slot);
        }
    }
}