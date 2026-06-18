using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandList : MonoBehaviour //Aquí se almacenan todas las funciones de comandos, se irán añadiendo cuando sea necesario
{
    public VillagerSpawner VillagerManager; //Para acceder al manager de los aldeanos, donde se almacena la lista de aldeanos
    public VillagerLogic selectedVillager; //Para almacenar al aldeano seleccionado
    public InventoryUI inventoryUI; //El sistema de inventario y su control visual
    public VillagersUI villagersUI; //El sistema de visualización de info de los aldeanos
    public VillagerInfoUI panelInfo; //EL panel para la info específica de un aldeano
    public GameObject pausePanel; //EL panel que se muestra cuando el juego entra en pausa
    public GameObject orderPanel; //El panel con la informacion sobre comandos del juego
    public TextMeshProUGUI resourceText; //La caja de texto para indicar los items necesarios
    public TextMeshProUGUI conditionText; //La caja de texto para indicar las condiciones de construcción necesarias

    public BuildingPlacer BP; //La lógica para colocar edificios
    private BuildingID selectedBuilding; //El ID del edificio a construir
    private ItemID selectedItem; //El ID del item a construir
    private bool waitingClick = false; //La variable para activar otro modo de espera para un click

    public ItemCreator IC; //LA logica para crear edificios
    private bool waitingCraftConfirm = false; //Segunda fase, cuando ya se sabe y se requiere confirmación

    public ResourceGenerator RG; //La lógica de generación de recursos para sabeer distinguir entre tipos de recursos
    public ResourceManager RM; //La lógica de adición de recursos para cuando se deba recolectar un recurso
    private bool waitingCollectClick = false; //La variable para dejar en modo de espera de un click sobre un recurso

    private bool waitingRepairClick = false; //La variable para saber cuando se espera un click sobre un edificio destruido
    private bool waitingDestroyClick = false; //La variable para ponerse en modo de espera de un edificio que destruir
    private bool waitingCombatClick = false; //La variabl para cuando se espera a qu cliques un enemigo

    public void SelectVillager(string name) //Función para escoger un aldeano, en base al nombre se elige
    {
        foreach (VillagerLogic villager in VillagerManager.allVillagers)
        {
            if (villager.villagerName.Trim().ToLower() == name.Trim().ToLower())
            {
                selectedVillager = villager;
                Debug.Log("Aldeano seleccionado: " + selectedVillager.villagerName);
                return;
            }
        }
        Debug.Log("No se ha encontrado al aldeano con el nombre: " + name);
    }

    public void TalkVillager(string name)//Función para que un aldeano seleccionado vaya a hablar con otro, en base a esto establecen una relación
    {
        VillagerLogic target = null;

        foreach (VillagerLogic villager in VillagerManager.allVillagers)
        {
            if (villager.villagerName.Trim().ToLower() == name.Trim().ToLower())
            {
                target = villager;
                break;
            }
        }

        if (selectedVillager == null || target == null)
        {
            Debug.Log("Se ha ejecutado, pero no existía el target o o no habías seleccionado a nadie");
            return;
        }
        else
        {
            Coroutine routine = StartCoroutine(ImproveRelationship(selectedVillager, target)); //Se ejecuta la corutina en la que ambos hablan

            selectedVillager.currentCoroutine = routine;
            target.currentCoroutine = routine;
        }
    }

    private IEnumerator ImproveRelationship(VillagerLogic a, VillagerLogic b) //Corutina para los dos aldeanos hablando
    {
        a.isBusy = true; //Se bloquea el movimiento libre de ambos 
        b.isBusy = true;

        while (true)
        {
            if (!a.isBusy || !b.isBusy)
            {
                a.isBusy = false;
                b.isBusy = false;
                yield break;
            }

            float dist = Vector3.Distance(a.transform.position, b.transform.position);
            if (dist <= 0.4f) break;
            a.transform.position = Vector3.MoveTowards(a.transform.position,b.transform.position,Time.deltaTime * 1f); //El aldeano seleccioando se mueve a la posición del otro
            yield return null;
        }

        float timer = 15f;
        while (timer > 0f) //Se quedan "hablando el tiempo limite, pero dando la posibilidad de que se cancele la rutina"
        {
            if (!a.isBusy || !b.isBusy)
            {
                a.isBusy = false;
                b.isBusy = false;
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        // Si ninguno de los dos tenia relacion aun entran en una relacion
        bool aFree = (a.relationship == null || (!a.relationship.inLove && a.relationship.LoverID == 0));
        bool bFree = (b.relationship == null || (!b.relationship.inLove && b.relationship.LoverID == 0));

        if (aFree && bFree)
        {
            a.relationship = new Relationship
            {
                LoverID = b.id,
                inLove = true
            };

            b.relationship = new Relationship
            {
                LoverID = a.id,
                inLove = true
            };
        }

        a.isBusy = false;
        b.isBusy = false;
    }

    public void VillagerGreets() //El aldeano te saluda, no hace nada más
    {
        if (selectedVillager)
        {
            Coroutine routine = StartCoroutine(Greeting(selectedVillager));
            selectedVillager.currentCoroutine = routine;
        }
    }

    private IEnumerator Greeting(VillagerLogic villager) //Controla el tiempo que saluda el aldeano
    {
        villager.isBusy = true;
        yield return new WaitForSeconds(3);
        villager.isBusy = false;
    }

    public void AssignWorker(string factoryName) //Función para asignar trabajadores
    {
        if (!System.Enum.TryParse(factoryName, true, out BuildingID targetID))
        {
            Debug.LogWarning("No se pudo parsear el tipo de fábrica: " + factoryName);
            return;
        }

        FactoriesLogic closestFactory = null; //LA fabrica mas cercana
        float closestDistance = Mathf.Infinity; //calcula la distancia a una fabrica

        FactoriesLogic[] allFactories = FindObjectsOfType<FactoriesLogic>(); //Busca todas las fabricas creadas

        foreach (FactoriesLogic factory in allFactories)
        {
            if (!factory.HasFreeWorkerSpace()) continue; //Se elimina de la busqueda si la fabrica no tiene huecos de trabajo disponibles
            if (factory.buildingID != targetID) continue; //O si no es el tipo de fabrica solicitada
            float distance = Vector3.Distance(selectedVillager.transform.position, factory.transform.position); //Se busca la posición posible más cercana
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestFactory = factory;
            }
        }

        if (closestFactory == null)
        {
            Debug.LogWarning("No se encontró ninguna fábrica válida para: " + factoryName);
            return;
        }
        Coroutine routine = StartCoroutine(WorkRoutine(selectedVillager, closestFactory)); //Se pone al aldeano a trabajar
        selectedVillager.currentCoroutine = routine;
    }

    private IEnumerator WorkRoutine(VillagerLogic villager, FactoriesLogic factory)
    {
        if (villager == null || factory == null)
        {
            Debug.LogError("WorkRoutine recibió referencias nulas");
            yield break;
        }

        if (villager.energyPoints <= 0) yield break;

        villager.isBusy = true;

        SpriteRenderer sr = selectedVillager.GetComponent<SpriteRenderer>();
        Collider col = selectedVillager.GetComponent<Collider>();

        while (Vector3.Distance(villager.transform.position, factory.transform.position) > 0.5f) //El aldeano se va a la posición de la fabrica más cercana
        {
            if (!villager.isBusy)
            {
                RemoveFromFactory(villager);
                yield break;
            }
            villager.transform.position = Vector3.MoveTowards(villager.transform.position, factory.transform.position, Time.deltaTime);
            yield return null;
        }

        factory.assignedWorkers.Add(selectedVillager); //Se suma uno al hueco de trabajadores
        factory.workersAssigned++;
        villager.currentFactory = factory;

        //Se desactivan el sprite y el collider del aldeano para dar a entender que se ha desactivado
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        while (villager.energyPoints > 0) //El trabajador trabaja mientras tenga energía, cada hora le quita uno a su energía
        {
            if (!villager.isBusy)
            {
                RemoveFromFactory(villager);

                if (sr != null) sr.enabled = true;
                if (col != null) col.enabled = true;

                yield break;
            }
            yield return new WaitForSeconds(60f);
            villager.energyPoints--;
        }

        RemoveFromFactory(villager);
        factory.workersAssigned--; //Al terminar de trabajar, se libera el puesto en la fabrica
        
        //Y se vuelven a activar las componentes al terminar de trabajar
        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;

        villager.isBusy = false;
        villager.transform.position = factory.transform.position + Vector3.right;
    }

    public void StoreItems(string storageName) //Función para almacenar items en los almacenes
    {
        if (!System.Enum.TryParse(storageName, true, out Resources targetResource))
        {
            Debug.LogWarning("No se pudo parsear el recurso: " + storageName);
            return;
        }

        StorageLogic closest = null; //Como con las fabricas, e busca el más cercano con huecos libres aún
        float bestDist = Mathf.Infinity;

        StorageLogic[] all = FindObjectsOfType<StorageLogic>();

        foreach (var s in all)
        {
            if (!s.HasFreeSpace()) continue;
            if (s.data.storedMaterial != targetResource) continue;

            float d = Vector3.Distance(selectedVillager.transform.position, s.transform.position);

            if (d < bestDist)
            {
                bestDist = d;
                closest = s;
            }
        }

        if (closest != null)
        {
            Coroutine routine = StartCoroutine(StoreRoutine(selectedVillager, closest)); //Al encontrar el más cercano y llegar hasta el empieza la corutina de almacen
            selectedVillager.currentCoroutine = routine;
        }
    }

    private IEnumerator StoreRoutine(VillagerLogic villager, StorageLogic storage) //La corutina para almacenarlos
    {
        villager.isBusy = true; //El aldeano se desplaza hasta el almacen

        while (Vector3.Distance(villager.transform.position, storage.transform.position) > 0.3f)
        {
            villager.transform.position = Vector3.MoveTowards(
                villager.transform.position,
                storage.transform.position,
                Time.deltaTime
            );
            yield return null;
        }

        foreach (var slot in villager.inventory.slots) //Ahora comienza a almacenar todos los items correspondientes en el almacen
        {
            if (slot.IsEmpty()) continue; //Si el slot no lleva nada se avanza al siguiente

            if (!storage.CanStore(slot.item)) continue; //Si el item del slot no coincide con el que se puede almacenar en el almacen dicho se avanza al siguiente

            int stored = storage.inventory.AddItem(slot.item, slot.quantity); //Se almacenan los objetos

            if (stored > 0)
            {
                slot.item = null;
                slot.quantity = 0;
            }
        }

        villager.isBusy = false; //El aldeano vuelve a estar libre al terminar rapidamente
    }

    public void TakeItems(string itemName) //Función grande que se usará para sacar recursos e items
    {
        //Caso de sacar un recurso
        if (System.Enum.TryParse(itemName, true, out Resources resourceType))
        {
            TakeResource(resourceType);
            return;
        }

        //Caso de sacar un item
        if (System.Enum.TryParse(itemName, true, out ItemID itemID))
        {
            TakeSpecificItem(itemID);
            return;
        }

    }

    public void TakeResource(Resources type) //Se busca el almacen más cercano con ese recurso
    {
        StorageLogic closest = null;
        float bestDist = Mathf.Infinity;

        foreach (var s in FindObjectsOfType<StorageLogic>())
        {
            if (s.data.storedMaterial != type) continue;

            float d = Vector3.Distance(selectedVillager.transform.position, s.transform.position);

            if (d < bestDist)
            {
                bestDist = d;
                closest = s;
            }
        }

        if (closest != null)
        {
            Coroutine routine = StartCoroutine(TakeResourceRoutine(selectedVillager, closest, type));
            selectedVillager.currentCoroutine = routine;
        }
    }

    private IEnumerator TakeResourceRoutine(VillagerLogic villager, StorageLogic storage, Resources type) //La corutina para sacar un stack completo o la cantidad máxima disponible del recurso
    {
        villager.isBusy = true;

        while (Vector3.Distance(villager.transform.position, storage.transform.position) > 0.3f)
        {
            villager.transform.position = Vector3.MoveTowards(villager.transform.position,storage.transform.position,Time.deltaTime);
            yield return null;
        }

        for (int i = 0; i < storage.inventory.slots.Count; i++)
        {
            var slot = storage.inventory.slots[i];

            if (slot.IsEmpty()) continue;
            if (slot.item.resourceType != type) continue;

            int amount = Mathf.Min(10, slot.quantity);

            int added = villager.inventory.AddItem(slot.item, amount);

            if (added > 0)
            {
                slot.quantity -= amount;

                if (slot.quantity <= 0)
                    storage.inventory.ClearSlot(i);

                break;
            }
        }

        villager.isBusy = false;
    }

    void TakeSpecificItem(ItemID id) //Se busca el almacen con el tipo de herramienta o arma pedido más cercano
    {
        StorageLogic closest = null;
        float bestDist = Mathf.Infinity;

        foreach (var s in FindObjectsOfType<StorageLogic>())
        {
            float d = Vector3.Distance(selectedVillager.transform.position, s.transform.position);

            if (d < bestDist)
            {
                bestDist = d;
                closest = s;
            }
        }

        if (closest != null)
        {
            Coroutine routine = StartCoroutine(TakeItemRoutine(selectedVillager, closest, id));
            selectedVillager.currentCoroutine = routine;
        }
    }

    private IEnumerator TakeItemRoutine(VillagerLogic villager, StorageLogic storage, ItemID id) //Se saca el item en específico
    {
        villager.isBusy = true;

        while (Vector3.Distance(villager.transform.position, storage.transform.position) > 0.3f)
        {
            villager.transform.position = Vector3.MoveTowards(
                villager.transform.position,
                storage.transform.position,
                Time.deltaTime
            );
            yield return null;
        }

        for (int i = 0; i < storage.inventory.slots.Count; i++)
        {
            var slot = storage.inventory.slots[i];

            if (slot.IsEmpty()) continue;
            if (slot.item is not ItemData itemData) continue;
            if (itemData.id != id) continue;

            int added = villager.inventory.AddItem(slot.item, 1);

            if (added > 0)
            {
                slot.quantity--;

                if (slot.quantity <= 0)
                    storage.inventory.ClearSlot(i);

                break;
            }
        }

        villager.isBusy = false;
    }

    public void CloseAll()
    {
        inventoryUI.CloseInventory();
        villagersUI.CloseUI();
        panelInfo.Hide();
        pausePanel.SetActive(false);
        orderPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenInventory()
    {
        CloseAll();
        inventoryUI.OpenInventory(selectedVillager);
    }

    public void OpenInfo()
    {
        CloseAll();
        villagersUI.OpenUI();
    }

    public void OpenVillager()
    {
        CloseAll();
        panelInfo.Show(selectedVillager);
    }

    public void OpenOrders()
    {
        CloseAll();
        orderPanel.SetActive(true);
    }

    public void PauseGame()
    {
        CloseAll();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SelectBuilding(string building) //Comando para escoger un edificio y entrar en el modo de espera de un click
    {
        if (!System.Enum.TryParse(building, true, out selectedBuilding))
        {
            Debug.Log("No se pudo parsear el edificio: " + building);
            return;
        }

        BuildingData data = BP.GetBuildingData(selectedBuilding);

        string costString = "Coste: "; //Info de los recursos de construcción necesarios
        foreach (var cost in data.productionCost)
        {
            costString += cost.material + "x" + cost.amount + " ";
        }
        resourceText.text = costString;

        string reqString = "Construir en: "; //Info del recurso sobre se debe construir
        foreach (var req in BP.buildingRequeriments)
        {
            if (req.id == selectedBuilding)
            {
                reqString += req.requiredResourceType;
                break;
            }
        }
        conditionText.text = reqString;

        StartCoroutine(HideBuildingInfo()); //Se enseña tres egundos y luego se esconde

        Debug.Log("Reconocido edificio: "+ selectedBuilding);
        waitingClick = true;
    }

    IEnumerator HideBuildingInfo() 
    {
        resourceText.gameObject.SetActive(true);
        conditionText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f); //Se enseña la info necesario durante 10 segundos antes de volver a ocultarla para que el jugador separ que necesita

        resourceText.gameObject.SetActive(false);
        conditionText.gameObject.SetActive(false);
    }

    void Update() //Para el modo de espera del edificio
    {
        if (waitingClick && Input.GetMouseButtonDown(0)) //Si recibe un click y estaba esperando click de edificio comenzará a construir el edificio en esa posición 
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0; //Se debe modificar ligeramente la posición del edificio en < para que coincida con el aldeano y asi el seguimiento de posicion funcione
            Vector3Int tilePos = BP.tilemap.WorldToCell(world);

            TryPlaceBuilding(tilePos, selectedBuilding);
        }
        if (waitingCollectClick && Input.GetMouseButtonDown(0)) //Si está esperando un recurso y se clica sobre este va a recolectarlo
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0; //De nuevo se modifica ligeramente la posición del recurso en z para que así coincida con el aldeano
            Vector3Int tilePos = BP.tilemap.WorldToCell(world);

            TryCollect(tilePos);
            waitingCollectClick = false;
        }
        if (waitingRepairClick && Input.GetMouseButtonDown(0)) //S se está esperando un edificio que debe ser reparado, se registrará la posición clicada y se actuará
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            Vector3Int tilePos = BP.tilemap.WorldToCell(world);

            TryRepair(tilePos);
            waitingRepairClick = false;
        }
        if (waitingDestroyClick && Input.GetMouseButtonDown(0))//Se esper un edificio que destruir al ser clicado
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            Vector3Int tilePos = BP.tilemap.WorldToCell(world);

            TryDestroy(tilePos);
            waitingDestroyClick = false;
        }
        if (waitingCombatClick && Input.GetMouseButtonDown(0)) //Se espera a que le des click a un enemigo
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;
            Collider2D col = Physics2D.OverlapPoint(world);

            if (col != null)
            {
                EnemyLogic enemy = col.GetComponent<EnemyLogic>();

                if (enemy != null)
                {
                    CombatTarget target = enemy.GetComponent<CombatTarget>();

                    if (target != null)
                    {
                        Coroutine routine = StartCoroutine(CombatRoutine(selectedVillager, target));
                        selectedVillager.currentCoroutine = routine;
                    }
                }
            }

            waitingCombatClick = false;
        }
    }

    void TryPlaceBuilding(Vector3Int tilePos, BuildingID id) //La función para construir el edificio en sí
    {
        if (!IsValidPosition(tilePos, id)) return; //Se comprueba que el edificio esta en una posición correcta
        
        bool succes = BP.TryPlacingBuilding(id, tilePos); //Si es correcta la posición se inicia la construccion

        if (succes)
        {
            Coroutine routine = StartCoroutine(BuildingWorkRoutine(tilePos, id));
            selectedVillager.currentCoroutine = routine;
            waitingClick = false;
        }
    }

    bool IsValidPosition(Vector3Int pos, BuildingID id) //La comprobación de que el edificio está en posición correcta
    {
        var buildings = BP.GetPlacedBuildings();

        if (buildings.Count == 0) return true; //Por si acaso nos asguramos de que el primer edificio no tenga condiciones

        int closestDistance = int.MaxValue;

        foreach (var placed in buildings)
        {
            Vector3Int otherPos = placed.Key;
            int dist = Mathf.Abs(pos.x - otherPos.x) + Mathf.Abs(pos.y - otherPos.y); 
            if (dist < closestDistance) closestDistance = dist;
        }

        if (id == BuildingID.Altar) //Si el edificio es un altar se debe construir lejos de los demas edificio
        {
            return closestDistance >= 5;
        }
        else //Si no debe estar cerca de otros dos edificios
        {
            return closestDistance <= 2;
        }
    }

    IEnumerator BuildingWorkRoutine(Vector3Int pos, BuildingID id) //Rutina principal de trabajo
    {
        if (selectedVillager.energyPoints <= 0) yield break;
        Vector3 target = BP.tilemap.GetCellCenterWorld(pos); //Se encuentra la posición del edifio que se va a construir
        target.z = selectedVillager.transform.position.z;
        selectedVillager.isBusy = true;

        SpriteRenderer sr = selectedVillager.GetComponent<SpriteRenderer>();
        Collider col = selectedVillager.GetComponent<Collider>();

        while (Vector3.Distance(selectedVillager.transform.position, target) > 1f) //Se mueve el aldeano seleccionado hasta la posición cercana
        {
            selectedVillager.transform.position = Vector3.MoveTowards(selectedVillager.transform.position,target,Time.deltaTime * 2f);
            yield return null;
        }

        BuildingData data = BP.GetBuildingData(id);

        //Se desactiva el sprite y el collider del aldeano
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false; 

        yield return new WaitForSeconds(data.productionTime * 60f);//Trabaja durante el tiempo de producción del edificio

        //Y se vuelve a activar cuando ha terminado de trabajar
        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;

    
        selectedVillager.transform.position = target + Vector3.right; //Al terminar el aldeano aparece donde el edificio nuevo

        int hours = Mathf.CeilToInt(data.productionTime); //Se le quitan al aldeano tantos puntos de energía como horas de trabajo haya costado cosntruir o toda si costaba más que las horas de trabajo
        int energyCost = Mathf.Min(hours, selectedVillager.energyPoints);

        selectedVillager.energyPoints -= energyCost;

        selectedVillager.isBusy = false;
    }

    public void SelectCraft(string craft) //La primera fase del proceso de crafteo de items
    {
        Debug.Log("Iniciado crafteo");
    }

    public void SelectItem(string item) //El segundo paso del proces de crafteo
    {
        if (!System.Enum.TryParse(item, true, out ItemID itemID)) //Se comprueba qu exista el item y se parsean los datos
        {
            Debug.Log("Item no reconocido: " + item);
            return;
        }

        selectedItem = itemID;

        ItemData data = IC.RM.allItems.Find(i => i.id == selectedItem); //Se sacan los datos del item

        string costString = "Coste: ";
        foreach (var cost in data.productionCost)
        {
            costString += cost.material + "x" + cost.amount + " ";
        }
        resourceText.text = costString;
        StartCoroutine(ShowItemInfo()); //Y se muestran en pantalla
        waitingCraftConfirm = true;
    }

    IEnumerator ShowItemInfo() //La corrutina que lo controla
    {
        resourceText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        resourceText.gameObject.SetActive(false);
    }

    public void ConfirmCraft() //Ultimo paso
    {
        ItemData data = IC.RM.allItems.Find(i => i.id == selectedItem);
        if (!waitingCraftConfirm || data == null) return; //Al confirmarse el crafteo se lanza la corrutina de generacion

        Coroutine routine = StartCoroutine(CraftRoutine(selectedVillager, data));
        selectedVillager.currentCoroutine = routine;

        waitingCraftConfirm = false;
    }

    IEnumerator CraftRoutine(VillagerLogic villager, ItemData item)
    {
        if (villager.energyPoints <= 0) yield break;
        villager.isBusy = true;

        FactoriesLogic closest = IC.GetClosestBlacksmith(villager.transform.position); //El aldeano se va a la herreria más cercana

        if (closest == null)
        {
            Debug.LogWarning("No hay herrería disponible");
            villager.isBusy = false;
            yield break;
        }

        while (Vector3.Distance(villager.transform.position, closest.transform.position) > 0.5f)
        {
            villager.transform.position = Vector3.MoveTowards(villager.transform.position,closest.transform.position,Time.deltaTime);
            yield return null;
        }

        SpriteRenderer sr = villager.GetComponent<SpriteRenderer>();
        Collider col = villager.GetComponent<Collider>();
        if (sr != null) sr.enabled = false; //Al llegar se desactiva y se pone a "trabajar"
        if (col != null) col.enabled = false;

        if (!IC.RM.HasResources(item.productionCost)) // Si no hay recursos se detiene el proceso
        {
            Debug.LogWarning("No hay recursos suficientes para craftear");
            
            if (sr != null) sr.enabled = true;
            if (col != null) col.enabled = true;

            villager.isBusy = false;
            yield break;
        }

        IC.TryProduceItem(item);
        yield return new WaitForSeconds(item.productionTime * 60f);//Se deja al aldeano trabajando el tiempo necesario

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true; //Se vueleve a activar
        villager.transform.position = closest.transform.position + Vector3.right;

        int hours = Mathf.CeilToInt(item.productionTime); //Y se le quita la energia correspondiente a las horas de trabajo o el máximo de energía que tuviera
        if (hours < 1) //Si el crafteo era corto y duraba menos de una hora, se le quita uno de energía.
        {
            hours = 1;
        }
        int energyCost = Mathf.Min(hours, villager.energyPoints);
        villager.energyPoints -= energyCost;

        villager.isBusy = false;
    }

    public void EquipItem(string name) //Metodo principal para equipar items a los aldeanos
    {
        if (selectedVillager == null) return;
        if (!System.Enum.TryParse(name, true, out ItemID itemID)) return; //Se extrae el item en base a su id

        foreach (var slot in selectedVillager.inventory.slots) //Se revisan los slots de inventario del aldeano para asegurar que este posee el item
        {
            if (slot.IsEmpty()) continue;
            if (slot.instance == null) continue;

            ItemInstance item = slot.instance;

            if (item.data.id == itemID) //Si estaba se equipa el item
            {
                if (!item.data.isEquipable) return; //Solo si este cuenta con la propiedad de ser equipable, que hemos dado solo a las herramientas y armas

                if (selectedVillager.equipedItem != null)
                    selectedVillager.equipedItem.isEquiped = false; //Si ya tenía un item equipado se desequipa

                selectedVillager.equipedItem = item;
                item.isEquiped = true;
                return;
            }
        }
    }

    public void StartCollectMode() //Primer metodo para la mecanica de recolección 
    {
        if (selectedVillager == null) return;

        waitingCollectClick = true; //Simplemente activa el modo de espera de un click sobre un recurso
    }

    public void TryCollect(Vector3Int tilePos)
    {
        GameObject resource = RG.GetResourceAt(tilePos); //Se extrae el recurso que está en la casilla clicada

        if (resource == null) return;

        FountainResourceLogic fountain = resource.GetComponent<FountainResourceLogic>(); //Si lo que hay es un recurso de tipo fuente se usa la corutina de extracción para fuentes
        if (fountain != null)
        {
            Coroutine routine = StartCoroutine(CollectFountain(selectedVillager, fountain));
            selectedVillager.currentCoroutine = routine;
            return;
        }

        TemporaryResourcesLogic temp = resource.GetComponent<TemporaryResourcesLogic>(); //Y si hay uno temporal se usa otra corutina con efectos diferentes
        if (temp != null)
        {
            Coroutine routine = StartCoroutine(CollectTemporary(selectedVillager, temp, tilePos));
            selectedVillager.currentCoroutine = routine;
            return;
        }
    }

    IEnumerator CollectFountain(VillagerLogic villager, FountainResourceLogic source) //LA corrutina de extracción de recursos de fuentes
    {
        if (villager.energyPoints <= 0) yield break;  // No se ejecuta si el aldeano ya no tiene energía
        
        villager.isBusy = true;

        while (source != null && Vector3.Distance(villager.transform.position, source.transform.position) > 0.5f)
        {
            if (!villager.isBusy) yield break;
            villager.transform.position = Vector3.MoveTowards(villager.transform.position, source.transform.position, Time.deltaTime * 2f);
            yield return null;
        }

        if (source == null)  //Tampoco si la fuente de recursos no existe
        { 
            villager.isBusy = false; 
            yield break; 
        }

        Debug.Log($"[INFO] Recurso: {source.resourceName} | Vida: {source.life} | Energía: {villager.energyPoints}");

        if (source.life <= 0) //O si esta ya no tiene energía
        {
            Debug.LogWarning("La fuente no tiene vida para recolectar.");
            villager.isBusy = false;
            yield break;
        }

        SpriteRenderer sr = villager.GetComponent<SpriteRenderer>();
        Collider col = villager.GetComponent<Collider>();
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        while (villager.isBusy && villager.energyPoints > 0 && source.life > 0) //Mientras el aldeano se mantenga ocupado, tenga energí y la fuente tenga vida se trabjara
        {
            int damage = Mathf.Max(1, villager.recolectDamage);  //Seguridad por si no se ha inicializado bien el daño de recolección del aldeano
            bool correctTool = false; //Y comprobación de que se está usando la herramienta adecuada

            if (villager.equipedItem != null && villager.equipedItem.data != null) //Para cada fuente esta varía en base al ID del item equipado
            {
                ItemID id = villager.equipedItem.data.id;
                switch (source.resourceType)
                {
                    case Resources.Agua: if (id == ItemID.Cubo) correctTool = true; break;
                    case Resources.Comida: if (id == ItemID.LanzaMadera || id == ItemID.LanzaPiedra || id == ItemID.LanzaMetal) correctTool = true; break;
                    case Resources.Madera: if (id == ItemID.Hacha || id == ItemID.HachaMetal) correctTool = true; break;
                    case Resources.Piedra: if (id == ItemID.Pala) correctTool = true; break;
                    case Resources.Metal: if (id == ItemID.Pico || id == ItemID.PicoMetal) correctTool = true; break;
                }

                if (correctTool) //Y si es correcta se aumenta el adaño de recolección diferenciando entre armas y herramientas
                {
                    if (villager.equipedItem.data is ToolData tool) damage += tool.effectivity;
                    if (villager.equipedItem.data is WeaponData weapon) damage += weapon.damage;
                    if (villager.equipedItem.HasDurability()) villager.equipedItem.Use();
                }
            }

            source.life -= damage;
            WorldResourceData resource = RM.GetResourceData(source.resourceType);
            
            if (resource != null) 
            {
                Debug.Log($"Intentando añadir: {resource.name} - Cantidad: {damage * source.resourcesPeLife}");
                villager.inventory.AddItem(resource, damage * source.resourcesPeLife); //Se añadirán tantos recursos como vida y recursos por vida haya perdido la fuente
            }
            
            villager.energyPoints--;

            if (source.life <= 0)
            {
                if (sr != null) sr.enabled = true;
                if (col != null) col.enabled = true; 
                villager.isBusy = false;
                source.StartRecovery();
                break;
            }

            yield return new WaitForSeconds(60f); // Tiempo entre cada extracción
        }

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
        villager.isBusy = false;
    }

    IEnumerator CollectTemporary(VillagerLogic villager, TemporaryResourcesLogic resource, Vector3Int pos) //La corrutina que gestiona la recolección de recursos temporales
    {
        if (villager.energyPoints <= 0) 
        { 
            Debug.LogWarning("Aldeano sin energía."); 
            yield break; 
        }
        
        villager.isBusy = true;

        while (resource != null && Vector3.Distance(villager.transform.position, resource.transform.position) > 0.6f)
        {
            if (!villager.isBusy) yield break;
            villager.transform.position = Vector3.MoveTowards(villager.transform.position, resource.transform.position, Time.deltaTime * 2.5f);
            yield return null;
        }

        if (resource == null) 
        { 
            villager.isBusy = false; 
            yield break; 
        }

        bool canCollect = false; //Funciona casi igual, aunque aquí se obliga a tener una herramienta para podrr actuar
        if (villager.equipedItem != null && villager.equipedItem.data != null)
        {
            ItemID id = villager.equipedItem.data.id;
            switch (resource.resourceKind)
            {
                case TemporaryResourceType.Matojo: if (id == ItemID.Pala) canCollect = true; break;
                case TemporaryResourceType.Animal: if (id == ItemID.LanzaMadera || id == ItemID.LanzaPiedra || id == ItemID.LanzaMetal) canCollect = true; break;
            }
        }

        if (!canCollect)
        {
            Debug.LogWarning("Herramienta no válida para este recurso temporal.");
            villager.isBusy = false;
            yield break;
        }

        WorldResourceData source = RM.GetResourceData(resource.resourceType);
        if (source != null) 
        {
            villager.inventory.AddItem(source, resource.value); //Aquí no se calcula nada, se dan los items valoreados
        }

        if (villager.equipedItem.HasDurability()) villager.equipedItem.Use();

        RG.RemoveResource(pos); 
        Destroy(resource.gameObject); //Y se detsruye el recurso temporal

        villager.energyPoints--;
        villager.isBusy = false;
    }

    public void StartRepairMode() //Primer metodo del sistema de reparación de edificios
    {
        if (selectedVillager == null) return;

        waitingRepairClick = true; //Que de nuevo, solo activa ell modo de espera de un click
    }

    void TryRepair(Vector3Int pos) //El metodo de comprobación de reparación
    {
        GameObject obj = RG.GetResourceAt(pos); //Se busca lo que hay en la casilla
        if (obj == null) return;

        DestroyedLogic destroyed = obj.GetComponent<DestroyedLogic>(); //Si hay un edificio destruido inicia la corrutina
        if (destroyed == null) return;

        Coroutine routine = StartCoroutine(RepairRoutine(selectedVillager, destroyed, pos));
        selectedVillager.currentCoroutine = routine;
    }

    IEnumerator RepairRoutine(VillagerLogic villager, DestroyedLogic destroyed, Vector3Int pos) //Corrutina para reparar edificios
    {
        if (villager.energyPoints <= 0) yield break;
        villager.isBusy = true;

        SpriteRenderer sr = villager.GetComponent<SpriteRenderer>(); //Se desactiva como en lois trabajos las componentes del aldeano
        Collider col = villager.GetComponent<Collider>();

        if (!RM.HasResources(destroyed.data.rebuildCost)) //Si no existen los recursos como para reparar el edificio se bloquea la acción
        {
            villager.isBusy = false;
            yield break;
        }

        while (Vector3.Distance(villager.transform.position, destroyed.transform.position) > 1f)
        {
            villager.transform.position = Vector3.MoveTowards(villager.transform.position,destroyed.transform.position,Time.deltaTime);
            yield return null;
        }

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        RM.SpendResources(destroyed.data.rebuildCost); //Se consumen los recursos

        yield return new WaitForSeconds(destroyed.data.rebuildTime * 60f); //Se incia la corutina de reparación durante el tiempo determinado

        BuildingData original = destroyed.GetOriginalBuilding(); //Al terminar la corrutina se extraen los dato del edificio original
        Vector3 worldPos = destroyed.transform.position;

        Destroy(destroyed.gameObject); //Se destruye el pefab de edificio destruido
        RG.RemoveResource(pos); //Se elimina del registro

        GameObject newBuilding = Instantiate(original.prefab, worldPos, Quaternion.identity); //Y se instancia una copia del edificio original
        BuildingsLogic logic = newBuilding.GetComponent<BuildingsLogic>();
        logic.Initialize(original);

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;

        int hours = Mathf.CeilToInt(destroyed.data.rebuildTime); //Se le quitan al aldeano tantos puntos de energía como horas de trabajo haya costado cosntruir o toda si costaba más que las horas de trabajo
        int energyCost = Mathf.Min(hours, selectedVillager.energyPoints);

        selectedVillager.energyPoints -= energyCost;

        villager.isBusy = false;
    }

    public void StartDestroyMode() //Primer paso para la destrucción de edificios
    {
        if (selectedVillager == null) return;

        waitingDestroyClick = true;
    }

    public void TryDestroy(Vector3Int pos) //Segundo paso para el metodo de destrucción
    {
        GameObject obj = RG.GetResourceAt(pos); //Se extraen los datos de la posicion

        if (obj != null)
        {
            Destroy(obj); //Si en esta hay algo se elimina el prefab y los datos sobre este en l casilla
            RG.RemoveResource(pos);
            return;
        }

        Collider2D col = Physics2D.OverlapPoint(BP.tilemap.GetCellCenterWorld(pos));

        if (col != null)
        {
            BuildingsLogic b = col.GetComponent<BuildingsLogic>();
            if (b != null)
            {
                Destroy(b.gameObject);
            }
        }
    }

    public void StartCooking() //Principal metodo para cuando toca cocinar
    {
        if (selectedVillager == null) return;
        if (selectedVillager.equipedItem == null || selectedVillager.equipedItem.data == null) return;
        if (selectedVillager.equipedItem.data.id != ItemID.Sarten) return; //Nos aseguramos de que el aldeano tiene equipada una sarten
        if (selectedVillager.energyPoints <= 0) return; //La acción no serealiza si el aldeano no tiene energía

        int food = CountFood(selectedVillager); //Y de que el aldeano tiene la comida necesaria
        if (food <= 0) return;
        Coroutine routine = StartCoroutine(CookRoutine(selectedVillager));
        selectedVillager.currentCoroutine = routine;
    }

    int CountFood(VillagerLogic villager) //MEtodo para registrar la comida que llega el aldeano en su inventario
    {
        int total = 0;

        foreach (var slot in villager.inventory.slots)
        {
            if (slot.IsEmpty()) continue;
            if (slot.item.resourceType != Resources.Comida) continue; //Si en el slot de inventario no tiene comida no se cuenta

            total += slot.quantity; //La suma de cantidades en slots con comida es el total
        }

        return total;
    }

    void RemoveFood(VillagerLogic villager, int amount) //Metodo para eliminar la comida en excesao que no se puede almacenar
    {
        for (int i = 0; i < villager.inventory.slots.Count; i++)
        {
            var slot = villager.inventory.slots[i];

            if (slot.IsEmpty()) continue;
            if (slot.item.resourceType != Resources.Comida) continue;

            int removed = Mathf.Min(slot.quantity, amount);
            slot.quantity -= removed;
            amount -= removed;

            if (slot.quantity <= 0)
                villager.inventory.ClearSlot(i);

            if (amount <= 0) break;
        }
    }

    IEnumerator CookRoutine(VillagerLogic villager) //Rutina principal de cocinado
    {
        villager.isBusy = true; //Se deja quieto al aldeano

        int initialFood = Mathf.Min(10, CountFood(villager)); //La comida inicial será como máximo un stack
        yield return new WaitForSeconds(60f);
        int totalAfter = initialFood * 2; //El resultado es el doble
        RemoveFood(villager, initialFood); //Por ello se elimina el stack original
        WorldResourceData foodItem = RM.GetResourceData(Resources.Comida);

        int remaining = totalAfter;

        foreach (var slot in villager.inventory.slots) //Se asigna la comida a los slots con hueco
        {
            if (slot.IsEmpty())
            {
                int add = Mathf.Min(10, remaining);
                slot.item = foodItem;
                slot.quantity = add;
                remaining -= add;
            }
            else if (slot.item.resourceType == Resources.Comida)
            {
                int space = 10 - slot.quantity;
                if (space <= 0) continue;

                int add = Mathf.Min(space, remaining);
                slot.quantity += add;
                remaining -= add;
            }

            if (remaining <= 0) break;
        }

        villager.energyPoints = Mathf.Max(0, villager.energyPoints - 1); //Y se le quita uno de nergía
        villager.isBusy = false;
    }

    public void StartEating()
    {
        if (selectedVillager == null) return;
        int food = CountFood(selectedVillager); //Nos aseguramos de que el aldeano tiene comida
        if (food <= 0) return;
        Coroutine routine = StartCoroutine(EatingTime(selectedVillager));
        selectedVillager.currentCoroutine = routine;
    }

    IEnumerator EatingTime(VillagerLogic villager) //Rutina para comer
    {
        villager.isBusy = true;

        int food = CountFood(villager);
        int toEat = Mathf.Min(10,food); //Se determina la comida que seva a comer, un máximo de un stack

        yield return new WaitForSeconds(60f); //Se espera durante una hora

        RemoveFood(villager, toEat); //Se elimina la comida del inventario
        villager.energyPoints += toEat; //Y se suma tanta energía como comida se haya consumido

        villager.isBusy = false;
    }

    void RemoveFromFactory(VillagerLogic villager) //Metodo para desasignar a un trbajador de la fabrica y solucinar el error de que se quedan asignados al cancelar su rutina
    {
        if (villager.currentFactory == null) return;

        if (villager.currentFactory.assignedWorkers.Contains(villager))
        {
            villager.currentFactory.assignedWorkers.Remove(villager);
            villager.currentFactory.workersAssigned--;
        }

        villager.currentFactory = null;
    }

    public void StopAction(VillagerLogic villager) //Metodo para parar la corutina de un aldeano
    {
        if (villager == null) return;

        Coroutine routine = villager.currentCoroutine;

        if (routine == null) return; //Si este está realizando una, se detiene
            
        foreach (VillagerLogic v in VillagerManager.allVillagers) //Se buscan a todos los aldeanos relizando esa corutina y se detiene
        {
            if (v.currentCoroutine == routine)
            {
                if (v.currentCoroutine != null)
                {
                    StopCoroutine(v.currentCoroutine);
                }
                v.currentCoroutine = null;
                v.isBusy = false;
                RemoveFromFactory(v);

                SpriteRenderer sr = v.GetComponent<SpriteRenderer>();
                Collider col = v.GetComponent<Collider>();

                if (sr != null) sr.enabled = true;
                if (col != null) col.enabled = true;
            }
        }
    }

    public void CancelAction() //Metodo que se invoca para evitar errores por tener que invocar también desde WordRegister al aldeano seleccionado
    {
        if (selectedVillager == null) return;
        StopAction(selectedVillager);
    }

    public void Rest() //Metodo principal para que los aldeanos se vayan a descansar
    {
        if (selectedVillager == null) return;

        HomeLogic targetHome = selectedVillager.currentHome; //El aldeano se irá a su casa seleccionada a descansar

        if (targetHome == null) //Si no tiene ninguna, se busca una casa disponible donde se pueda registrar como suya
        {
            float bestDist = Mathf.Infinity;

            foreach (HomeLogic home in FindObjectsOfType<HomeLogic>())
            {
                if (!home.CanRegister(selectedVillager)) continue;

                float d = Vector3.Distance(selectedVillager.transform.position, home.transform.position);

                if (d < bestDist)
                {
                    bestDist = d; //El tipo va a la casa más cercana siempre
                    targetHome = home;
                }
            }

            if (targetHome != null)
            {
                targetHome.RegisterVillager(selectedVillager);
            }
        }

        if (targetHome == null)
        {
            Debug.Log("No hay casa disponible");
            return;
        }

        Coroutine routine = StartCoroutine(RestRoutine(selectedVillager, targetHome));
        selectedVillager.currentCoroutine = routine;
    }

    IEnumerator RestRoutine(VillagerLogic villager, HomeLogic home) //Rurina principal para ir a descansar del aldeano
    {
        villager.isBusy = true;

        while (Vector3.Distance(villager.transform.position, home.transform.position) > 1f) 
        {
            villager.transform.position = Vector3.MoveTowards(villager.transform.position,home.transform.position,Time.deltaTime);
            yield return null;
        }

        SpriteRenderer sr = villager.GetComponent<SpriteRenderer>();
        Collider col = villager.GetComponent<Collider>();

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(480f); // Descansa ocho horas

        int energyGain = 10; //Y si la completa recupera 10 de energía

        int food = 0;
        int water = 0;

        foreach (var slot in villager.inventory.slots) //Se revisa el inventario
        {
            if (slot.IsEmpty()) continue;

            if (slot.item.resourceType == Resources.Comida)
                food += slot.quantity;

            if (slot.item.resourceType == Resources.Agua)
                water += slot.quantity;
        }

        if (food >= 10) //Y recupera más energía si tiene los recursos adecuados
        {
            RemoveResourceFromInventory(villager, Resources.Comida, 10);
            energyGain += 10;
        }

        if (water >= 10)
        {
            RemoveResourceFromInventory(villager, Resources.Agua, 10);
            energyGain += 10;
        }

        villager.energyPoints += energyGain;

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;

        villager.isBusy = false;
    }

    void RemoveResourceFromInventory(VillagerLogic villager, Resources type, int amount) //El metodo auxiliar para que el aldeano seleccionado unicamente pierda resursos al descansar
    {
        for (int i = 0; i < villager.inventory.slots.Count; i++)
        {
            var slot = villager.inventory.slots[i];

            if (slot.IsEmpty()) continue;
            if (slot.item.resourceType != type) continue;

            int removed = Mathf.Min(slot.quantity, amount);
            slot.quantity -= removed;
            amount -= removed;

            if (slot.quantity <= 0)
                villager.inventory.ClearSlot(i);

            if (amount <= 0) break;
        }
    }

    public void HaveKids() //El metodo àra que los aldeanos tengan hijos
    {
        if (selectedVillager == null) return;
        if (selectedVillager.relationship == null || !selectedVillager.relationship.inLove) return; //Se busca que el aldeano tenga una relacion

        VillagerLogic partner = null;

        foreach (var v in VillagerManager.allVillagers)
        {
            if (v.id == selectedVillager.relationship.LoverID)
            {
                partner = v;
                break;
            }
        }

        if (partner == null) return;

        if (selectedVillager.currentHome == null) return; //Y una casa
        if (partner.currentHome != selectedVillager.currentHome) return;

        if (selectedVillager.isBusy || partner.isBusy) return; //Nos aseguramos deque el compañero no está trabajando

        Coroutine routine = StartCoroutine(ReproductionRoutine(selectedVillager, partner)); //Y se ejecuta la corrutina

        selectedVillager.currentCoroutine = routine;
        partner.currentCoroutine = routine;
    }

    IEnumerator ReproductionRoutine(VillagerLogic a, VillagerLogic b) //La corutina de reproduccion
    {
        a.isBusy = true;
        b.isBusy = true;

        HomeLogic home = a.currentHome;

        while (Vector3.Distance(a.transform.position, home.transform.position) > 1f || Vector3.Distance(b.transform.position, home.transform.position) > 1f)
        {
            a.transform.position = Vector3.MoveTowards(a.transform.position, home.transform.position, Time.deltaTime);
            b.transform.position = Vector3.MoveTowards(b.transform.position, home.transform.position, Time.deltaTime);
            yield return null;
        }

        SpriteRenderer srA = a.GetComponent<SpriteRenderer>();
        SpriteRenderer srB = b.GetComponent<SpriteRenderer>();
        Collider colA = a.GetComponent<Collider>();
        Collider colB = b.GetComponent<Collider>();

        if (srA != null) srA.enabled = false;
        if (srB != null) srB.enabled = false;
        if (colA != null) colA.enabled = false;
        if (colB != null) colB.enabled = false;

        yield return new WaitForSeconds(240f);

        Vector3 spawnPos = home.transform.position + Vector3.right;

        VillagerLogic child = VillagerManager.GenerateChild(spawnPos);

        if (a.gender == Gender.Hombre)
        {
            child.fatherID = a.id;
            child.motherID = b.id;
        }
        else
        {
            child.fatherID = b.id;
            child.motherID = a.id;
        }

        home.RegisterVillager(child);

        if (srA != null) srA.enabled = true;
        if (srB != null) srB.enabled = true;
        if (colA != null) colA.enabled = true;
        if (colB != null) colB.enabled = true;

        a.isBusy = false;
        b.isBusy = false;
    }

    public void StartAttack() //Primer metodo que se emplea para entrar en modo de combate
    {
        if (selectedVillager == null) return;

        waitingCombatClick = true;
    }

    IEnumerator CombatRoutine(VillagerLogic attacker, CombatTarget target)
    {
        if (attacker.energyPoints <= 0) yield break;

        attacker.isBusy = true;

        float attackSpeed = 2f;
        float range = 1.2f;

        ItemInstance item = attacker.equipedItem;

        if (item != null && item.data is WeaponData weapon)
        {
            attackSpeed = weapon.attackSpeed;
            range = weapon.attackRange;
        }

        Rigidbody2D rb = attacker.GetComponent<Rigidbody2D>();

        while (target != null)
        {
            if (!attacker.isBusy) yield break;

            float dist = Vector3.Distance(attacker.transform.position, target.GetTransform().position);

            if (dist > range)
            {
                if (rb != null)
                {
                    Vector3 direction = (target.GetTransform().position - attacker.transform.position).normalized;
                    rb.velocity = new Vector2(direction.x, direction.y) * 2f;
                }
                yield return new WaitForFixedUpdate(); // Espera al siguiente frame de físicas
            }
            else
            {
                if (rb != null) rb.velocity = Vector2.zero;

                CombatSystem.Attack(attacker, target);
                yield return new WaitForSeconds(attackSpeed);
            }
        }

        if (rb != null) rb.velocity = Vector2.zero;
        attacker.isBusy = false;
    }
}