using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class WordRegister : MonoBehaviour //Nuevo sistema para leer comandos
{
    KeywordRecognizer keywordRecognizer;

    Dictionary<string, Action> Actions = new Dictionary<string, Action>(); //Lista con todas las acciones y palabras que debe reconocer el sistema
    private HashSet<string> validNames = new HashSet<string>(); //Lista con todos los nombres de aldeanos que el juego debe saber reconocer.
    private HashSet<string> validFactories = new HashSet<string>(); //Lista con las fabricas validas para trabajar
    private HashSet<string> validStorages = new HashSet<string>(); //Lista con los edificios validos para almacenar items
    private HashSet<string> validItems = new HashSet<string>();
    private HashSet<string> validCrafts = new HashSet<string>();
    private HashSet<string> validBuildings = new HashSet<string>();
    public List<BuildingID> allowedFactories; //La foma de editar el listado de fabricas desde el editor
    public List<BuildingID> allowedStorages; //La forma de editar el listado de almacenes desde el editor

    public CommandList CL;
    private bool waitingName = false; //Variable para dejar en espera el sistema hasta que se reciba un nombre valido
    private bool waitingTargetName = false; //Variable para dejar en espera el sistema si se espera el nombre de un destinatario
    private bool waitingFactory = false; //Variable para dejar en espera el sistema cuando se necesita el nombre de una fábrica
    private bool waitingStorage = false; //Variable para cuando se tiene que dejar en espera el controlador hasta que rciba el nombre de un almacén
    private bool waitingItem = false; //Variable para cuando se busca un item que sacar del inventrio
    private bool waitingBuilding = false; //Variable para cuando se está esperando un nombre de un edificio que se debe construir
    private bool waitingCraft = false; //Variable para cuando toque introducir un crafteo de item
    private bool waitingYes = false; //Variable para saber cuando estamos en stado de espera de un crafteo
    private bool waitingEquip = false; //Variable de control para cuando estamos esperando un equipable

    [SerializeField] private AudioSource AS;
    [SerializeField] private AudioClip RS;

    void Start()
    {
        InitializeSystem(); //Inizializamos el sistema de reconocimiento de palabras
    }

    public void InitializeSystem()
    {
        
        LoadNames(); //Cargamos los nombres,
        LoadCommands(); //Y los comandos, para evitar errores añadimos ahora
        LoadFactories(); //Las fabricas,
        LoadStorages(); //Los almacenes,
        LoadBuildings(); //Los edificios restantes,
        LoadCrafts(); //Los items para crafteos y equipacion
        LoadItems(); //Y los items generales

        keywordRecognizer = new KeywordRecognizer(Actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WordRecognized;
        keywordRecognizer.Start();
    }

    void LoadNames()
    {
        List<string> allNames = NameManager.Instance.GetAllNames(); 
        foreach (string name in allNames) //Revisamos y añadimos todos los nombres, modificandolos para que se adecuen al formato del programa
        {
            string clean = name.Trim().ToLower();
            if (!validNames.Contains(clean)) //Si ya existen en la lista no se añaden
            {
                validNames.Add(clean);
            }
        }
    }

    void LoadFactories()
    {
        validFactories.Clear(); //Por si acaso se resetea el sistema para evitar problemas

        foreach (BuildingID id in allowedFactories)
        {
            string clean = id.ToString().ToLower(); //pasamos todos los ids a texto
            validFactories.Add(clean);

            if (!Actions.ContainsKey(clean))
            {
                Actions.Add(clean, () => {}); //Si no existían ya las palabras en el registro se añaden sin un comando en específico
            }
        }
    }

    void LoadStorages()
    {
        validStorages.Clear(); //Por si acaso se resetea el sistema para evitar problemas

        foreach (BuildingID id in allowedStorages)
        {
            string clean = id.ToString().ToLower(); //pasamos todos los ids a texto
            validStorages.Add(clean);

            if (!Actions.ContainsKey(clean))
            {
                Actions.Add(clean, () => {}); //Si no existían ya las palabras en el registro se añaden sin un comando en específico
            }
        }
    }

    void LoadItems()
    {
        validItems.Clear();

        foreach (Resources r in Enum.GetValues(typeof(Resources))) //Se añaden todos los recursos
        {
            string clean = r.ToString().ToLower();
            validItems.Add(clean);

            if (!Actions.ContainsKey(clean))
            {
                Actions.Add(clean, () => {});
            }
        }

        foreach (ItemID id in Enum.GetValues(typeof(ItemID))) //Y todos los tipos de item
        {
            string clean = id.ToString().ToLower();
            validItems.Add(clean);

            if (!Actions.ContainsKey(clean))
            {
                Actions.Add(clean, () => {});
            }
        }
    }

    void LoadBuildings()
    {
        validBuildings.Clear();

        foreach (BuildingID id in Enum.GetValues(typeof(BuildingID)))
        {
            string clean = id.ToString().ToLower();
            validBuildings.Add(clean);

            if (!Actions.ContainsKey(clean))
            {
                Actions.Add(clean, () => {});
            }
        }
    }

    void LoadCrafts()
    {
        validCrafts.Clear();

        foreach (ItemID id in Enum.GetValues(typeof(ItemID))) 
        {
            string clean = id.ToString().ToLower();
            validCrafts.Add(clean);
        }
    }

    void LoadCommands() //Aquí añadiremos todos los comandos y palabras que queramos que se reconozcan
    {
        Actions.Clear();//Por si acaso borramos todo lo que haya para evitar repeticiones y errores
        Actions.Add("habla", () =>
        {
            waitingTargetName = true;;
        });
        Actions.Add("elegir", () => 
        {
            waitingName = true;
        });
        Actions.Add("saluda", () =>
        {
            CL.VillagerGreets();
        });
        Actions.Add("trabaja", () =>
        {
            waitingFactory = true;
        });
        Actions.Add("almacena", () =>
        {
            waitingStorage = true;
        });
        Actions.Add("saca", () =>
        {
            waitingItem = true;
        });
        Actions.Add("inventario", () =>
        {
            CL.OpenInventory();
        });
        Actions.Add("lista", () =>
        {
            CL.OpenInfo();
        });
        Actions.Add("info", () =>
        {
            CL.OpenVillager();
        });
        Actions.Add("pausa", () =>
        {
            CL.PauseGame();
        });
        Actions.Add ("comandos", () =>
        {
            CL.OpenOrders();
        });
        Actions.Add("cerrar", () =>
        {
            CL.CloseAll();
        });
        Actions.Add("construye", () =>
        {
            waitingBuilding = true;
        });
        Actions.Add("fabrica", () =>
        {
           waitingCraft = true; 
        });
        Actions.Add("si", () =>
        {
            if (waitingYes)
            {
                CL.ConfirmCraft();
                waitingYes = false;
            }
        });
        Actions.Add("usa", () =>
        {
            waitingEquip = true;
        });
        Actions.Add("recolecta", () =>
        {
            CL.StartCollectMode();
        });
        Actions.Add("repara", () =>
        {
            CL.StartRepairMode();
        });
        Actions.Add("destruye", () =>
        {
            CL.StartDestroyMode();
        });
        Actions.Add("cocina", () =>
        {
            CL.StartCooking();
        });
        Actions.Add("come", () =>
        {
            CL.StartEating();
        });
        Actions.Add("para", () =>
        {
           CL.CancelAction(); 
        });
        Actions.Add("descansa", () =>
        {
            CL.Rest();
        });
        Actions.Add("hijos", () =>
        {
            CL.HaveKids();
        });
        Actions.Add("ataca", () =>
        {
            CL.StartAttack();
        });

        foreach (string name in validNames)
        {
            if (!Actions.ContainsKey(name)) //Se añade cada nombre si aun no estaba para que se pueda reconocer, pero no apareja ninguna acción aún
            {
                Actions.Add(name, () => {});
            }
        }
    }

    private void WordRecognized(PhraseRecognizedEventArgs word)
    {
        string recognized = word.text.ToLower().Trim();

        if (waitingName) //Si el estado es el de esperar un nombre, se comprueba el nombre y se selecciona al aldeano
        {
           if (validNames.Contains(recognized))
            {
                CL.SelectVillager(recognized);
                waitingName = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingTargetName) //Mismo caso, pero para cuando se espera un nombre de destino para hablar
        {
            if (validNames.Contains(recognized))
            {
                CL.TalkVillager(recognized);
                waitingTargetName = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingFactory)
        {
            if (validFactories.Contains(recognized))
            {
                CL.AssignWorker(recognized);
                waitingFactory = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingStorage)
        {
            if (validStorages.Contains(recognized))
            {
                CL.StoreItems(recognized);
                waitingStorage = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingItem)
        {
            if (validItems.Contains(recognized))
            {
                CL.TakeItems(recognized);
                waitingItem = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingBuilding)
        {
            if (validBuildings.Contains(recognized))
            {
                CL.SelectBuilding(recognized);
                waitingBuilding = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingCraft)
        {
            if (validCrafts.Contains(recognized))
            {
                CL.SelectCraft(recognized);
                waitingCraft = false;
                waitingYes = true;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        if (waitingEquip)
        {
            if (validCrafts.Contains(recognized))
            {
                CL.EquipItem(recognized);
                waitingEquip = false;
                Debug.Log("Reconocido comando payo: " + recognized);
                SoundEffect();
            }
            return;
        }

        //Si no está en modo de espera, buscará la palabra en el diccionario de comandos y actuará
        if (Actions.ContainsKey(recognized))
        {
            Actions[recognized].Invoke();
            Debug.Log("Reconocido comando payo: " + recognized);
            SoundEffect();
        }
        return;
        //Si no está, no hace nada
    }

    public void SoundEffect()
    {
        AS.PlayOneShot(RS);
    }
}