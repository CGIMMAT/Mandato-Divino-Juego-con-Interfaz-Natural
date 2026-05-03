using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour //El encargado de la parte visual del inventario
{
    public InventoryLogic inventory;
    public RectTransform background;
    public Transform slotsContainer;
    public GameObject slotPrefab;
    public GameObject inventoryPanel;

    public int columns = 5;
    public int slotSize = 150;
    public int spacing = 25;
    public int padding = 25;

    private GridLayoutGroup grid;

    void Awake()
    {
        grid = slotsContainer.GetComponent<GridLayoutGroup>();
        ConfigureGrid();
    }

    void ConfigureGrid()
    {
        grid.cellSize = new Vector2(slotSize,slotSize);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.spacing = new Vector2(spacing,spacing);
        grid.padding = new RectOffset(padding,padding,padding,padding);
    }

    public void GenerateInventoryUI()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory no asignado");
            return;
        }

        for (int i = slotsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(slotsContainer.GetChild(i).gameObject);
        }

        int rows = Mathf.CeilToInt((float)inventory.maxSlots/columns);
        float width = columns * slotSize + (columns - 1) * spacing + padding * 2;
        float height = rows * slotSize + (rows - 1) * spacing + padding * 2;
        background.sizeDelta = new Vector2(width, height);

        for (int i = 0; i < inventory.maxSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsContainer);
            InventoryUISlots slotUI = slot.GetComponent<InventoryUISlots>();

            if (slotUI != null)
            {
                slotUI.SetData(inventory.slots[i]);
            }
        }
    }

    public void OpenInventory(VillagerLogic villager)
    {
        if (villager == null)
        {
            Debug.LogError("No hay aldeano seleccionado");
            return;
        }

        inventory = villager.inventory;

        inventoryPanel.SetActive(true);

        GenerateInventoryUI();
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotsContainer.childCount; i++)
        {
            InventoryUISlots slotUI = slotsContainer.GetChild(i).GetComponent<InventoryUISlots>();

            if (slotUI != null && i < inventory.slots.Count)
            {
                slotUI.SetData(inventory.slots[i]);
            }
        }
    }

    void Update()
    {
        if (inventoryPanel.activeSelf)
        {
            RefreshUI();
        }
    }
}