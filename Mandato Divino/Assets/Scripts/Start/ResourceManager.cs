using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Se usará para poder modificar los contadores de recursos en la interfaz

public class ResourceManager : MonoBehaviour //Usaremos esta clase como contabilizador de los recursos de los juegadores y del
{
    public float time = 0f; //Contará los días de juego 
    public int population = 0;

    public TextMeshProUGUI foodCounter;
    public TextMeshProUGUI waterCounter;
    public TextMeshProUGUI woodCounter;
    public TextMeshProUGUI stoneCounter;
    public TextMeshProUGUI seedsCounter;
    public TextMeshProUGUI steelCounter;
    public TextMeshProUGUI timeCounter;
    public TextMeshProUGUI populationCounter;

    public List<WorldResourceData> allResources;
    public List<ItemData> allItems;

    void Update() //La interfaz cuenta en todo momento los recursos que nos quedan
    {
        time += Time.deltaTime;
        float hours = time/60f; //Una hora dentro de la partida equivaldrá a 60 segundos
        float days = hours/24f; //Un día de juego son 24 minutos reales

        Dictionary<Resources, int> totals = CalculateResources();

        foodCounter.text = "Comida: " + GetValue(totals, Resources.Comida);
        waterCounter.text = "Agua: " + GetValue(totals, Resources.Agua);
        woodCounter.text = "Madera: " + GetValue(totals, Resources.Madera);
        stoneCounter.text = "Piedra: " + GetValue(totals, Resources.Piedra);
        steelCounter.text = "Metales: " + GetValue(totals, Resources.Metal);
        populationCounter.text = "Siervos: " + population;
        timeCounter.text = "Dia " + days.ToString("F0") + " Hora " + hours.ToString("F0");
    }

    public Dictionary<Resources, int> CalculateResources() //Función que almacena la cantidad de recursos que posee el jugador
    {
        Dictionary<Resources, int> totals = new Dictionary<Resources, int>(); //El diccionario con todos los items que posee el jugador
        foreach (Resources r in System.Enum.GetValues(typeof(Resources))) //Se comprueban los ietmas en inventarios y almacenes 
        {
            totals[r] = 0;
        }

        foreach (VillagerLogic v in FindObjectsOfType<VillagerLogic>())
        {
            SumInventory(v.inventory, totals);
        }

        foreach (StorageLogic s in FindObjectsOfType<StorageLogic>())
        {
            SumInventory(s.inventory, totals);
        }

        return totals;
    }

    public bool HasResources(List<Cost> costs) //Funcioon auxiliar que usaran otros códigos para comprobar que el jugador tiene los items necesarios para una acción
    {
        Dictionary<Resources, int> totals = CalculateResources(); //Se calculan los recursos totales

        foreach (var cost in costs)
        {
            if (!totals.ContainsKey(cost.material) || totals[cost.material] < cost.amount) //Si no existen los recursos o la cantidad de recursos es menor no se hace nada
                return false;
        }

        return true;
    }

    public void SpendResources(List<Cost> costs) //Funcion para gastar recursos
    {
        foreach (var cost in costs)
        {
            RemoveResource(cost.material, cost.amount); //La invocarán las funciones de colocación de items y quitaran los recursos
        }
    }

    void SumInventory(InventoryLogic inv, Dictionary<Resources, int> totals) //Metodo de apoyo que suma los items totales en todos los slots de inventario
    {
        if (inv == null) return;

        foreach (var slot in inv.slots)
        {
            if (slot.IsEmpty()) continue;

            if (slot.item != null) 
            {
                Resources type = slot.item.resourceType;
                if (IsValidResource(type)) 
                {
                    totals[type] += slot.quantity;
                }
            }
        }
    }

    void RemoveResource(Resources type, int amount) //Función para la eliminación de recursos
    {
        foreach (var v in FindObjectsOfType<VillagerLogic>())
        {
            amount = RemoveFromInventory(v.inventory, type, amount);
            if (amount <= 0) return;
        }

        foreach (var s in FindObjectsOfType<StorageLogic>())
        {
            amount = RemoveFromInventory(s.inventory, type, amount);
            if (amount <= 0) return;
        }
    }

    int RemoveFromInventory(InventoryLogic inv, Resources type, int amount) //Función que elimina los recursos del inventario
    {
        if (inv == null) return amount;

        for (int i = 0; i < inv.slots.Count; i++)
        {
            var slot = inv.slots[i];

            if (slot.IsEmpty()) continue;
            if (slot.item is not WorldResourceData resource) continue;
            if (resource.resourceType != type) continue;

            int removed = Mathf.Min(slot.quantity, amount);
            slot.quantity -= removed;
            amount -= removed;

            if (slot.quantity <= 0)
                inv.ClearSlot(i);

            if (amount <= 0) break;
        }

        return amount;
    }

    bool IsValidResource(Resources type) //Comprobación de que el tipo de recurso es valido, si no es de los tipos que se usan como recurso de jugador se descarta
    {
        return type == Resources.Agua ||
               type == Resources.Comida ||
               type == Resources.Madera ||
               type == Resources.Piedra ||
               type == Resources.Metal; //Es necesario ya que también hay tipos de recurso herramienta y arma, las cuales no deben ser contadas para los contadores
    }

    int GetValue(Dictionary<Resources, int> dict, Resources type) //Para saber cantidades decara a la UI
    {
        if (dict.ContainsKey(type)) return dict[type];
        return 0; 
    }

    public int GetAmount(Resources type) //Para saber cantidades de base a logica
    {
        Dictionary<Resources, int> totals = CalculateResources();
        if (totals.ContainsKey(type)) return totals[type];
        return 0;
    }

    public void SpendSingle(Resources type, int amount)
    {
        RemoveResource(type, amount);
    }

    public ItemData GetItemData(ItemID id)
    {
        return allItems.Find(i => i.id == id);
    }
    
    public WorldResourceData GetResourceData(Resources type)
    {
        foreach (var resource in allResources)
        {
            if (resource.resourceType == type)
                return resource;
        }

        return null;
    }
}