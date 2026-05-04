using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerLogic : MonoBehaviour, CombatTarget //Datos que deberá tener el prefab del aldeano cuando se genere
{
    public string villagerName; //Su nombre
    public int id; //Su identficador

    public Age age; //Su edad
    public Gender gender; //Su genero, ambos se usaran para determianr su sprite

    public int lifePoints; //Su vida
    public int energyPoints; //Las acciones que puede hacer antes de descansar
    public int inventorySlots; //Los items que puede llevar

    public bool canWork; //Si puede trabajar
    public bool isBusy = false; //Si está ocupado
    public Relationship relationship; //Su relación y estado amoroso
    public int fatherID;
    public int motherID;

    public Sprite maleSprite; //Sprite cuando es hombre
    public Sprite femaleSprite; //Sprite cuando es mujer

    private SpriteRenderer SR; //Componente para cambiar el sprite del prefab
    public InventoryLogic inventory; //El inventario personal
    public ItemInstance equipedItem;

    public int attackDamage;
    public int recolectDamage;

    public Coroutine currentCoroutine;
    public FactoriesLogic currentFactory;
    public HomeLogic currentHome;

    void Awake()
    {
        SR = GetComponent<SpriteRenderer>(); //Inizializamos el componente del sprite para poder manipularlo con el siguiente metodo
        if (inventory == null)
        {
            inventory = new InventoryLogic(inventorySlots > 0 ? inventorySlots : 5);
        }
    }

    public void SpriteSelector() //Función para cambiar el sprit en base al genero. Más adelante se añadirá la función para cambar el sprite en base a la edad
    {
        if (gender == Gender.Hombre)
        {
            SR.sprite = maleSprite;
        }
        else
        {
            SR.sprite = femaleSprite;
        }
    }

    public void Initialize(VillagersData data, int newID, Age newAge)
    {
        id = newID;
        villagerName = data.characterName;

        lifePoints = data.lifePoints;
        energyPoints = data.energyPoints;
        inventorySlots = data.inventorySlots;

        attackDamage = data.attackDamage;
        recolectDamage = data.recolectDamage;

        age = newAge;
        gender = (Gender) Random.Range(0, System.Enum.GetValues(typeof(Gender)).Length); //Se asigna aleatoriamnete el genero de personaje
        villagerName = NameManager.Instance.GetRandomName(gender);
        canWork = (age != Age.Niño); //Identifica si el aldeano puede trabajar

        relationship = new Relationship //Una relación inicial para marcar que el aldeano aún no tiene pareja
        {
            LoverID = 0,
            inLove = false
        };

        fatherID = 0;
        motherID = 0;
        currentHome = null;

        SpriteSelector();
        inventory = new InventoryLogic(inventorySlots); //Inicializamos su inventario personal
    }

    public void TakeDamage(int amount) //metodo para que los personajes puedan recibir daño
    {
        lifePoints -= amount;
        
        if (lifePoints <= 0) //Y llegar a morir
        {
            Die();
        }
    }

    public void Die()
    {
        StopAllCoroutines();
        isBusy = false;
        Destroy(gameObject);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void UpdateInventorySlots()
    {
        if (inventory != null)
        {
            inventory.UpdateSpecialSlots();
        }
    }

    public bool isDead()
    {
        return lifePoints <= 0;
    }
}