using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VillagerSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI idText;

    private VillagerLogic villager;
    private VillagersUI manager;

    public void SetData(VillagerLogic v, VillagersUI uiManager)
    {
        villager = v;
        manager = uiManager;

        nameText.text = v.villagerName;
        idText.text = "ID: " + v.id;
    }
}