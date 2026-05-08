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

    public Sprite[] maleSprites; //Sprite cuando es hombre
    public Sprite[] femaleSprites; //Sprite cuando es mujer

    private SpriteRenderer SR; //Componente para cambiar el sprite del prefab
    public InventoryLogic inventory; //El inventario personal
    public ItemInstance equipedItem;

    public int attackDamage;
    public int recolectDamage;

    public Coroutine currentCoroutine;
    public FactoriesLogic currentFactory;
    public HomeLogic currentHome;

    private float ageTimer; //EL contador de tiempo que mide cuanto está vivo
    private const float ageDuration = 4320f; //3 días medidos en segundos, el tiempo que se tarda en envejecer

    void Awake()
    {
        SR = GetComponent<SpriteRenderer>(); //Inizializamos el componente del sprite para poder manipularlo con el siguiente metodo
        if (inventory == null)
        {
            inventory = new InventoryLogic(inventorySlots > 0 ? inventorySlots : 5);
        }
    }
    public void Update()
    {
        GrowOlder();
    }

    public void SpriteSelector() //Función para cambiar el sprit en base al genero. Más adelante se añadirá la función para cambar el sprite en base a la edad
    {
        int ageIndex = (int)age;

        if (gender == Gender.Hombre)
        {
            SR.sprite = maleSprites[ageIndex];
        }
        else if (gender == Gender.Mujer)
        {
            SR.sprite = femaleSprites[ageIndex];
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
        StatsUpdate();
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
        VillagerSpawner spawner = FindObjectOfType<VillagerSpawner>();
        if (spawner != null) spawner.RemoveVillager(this);
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

    public void GrowOlder()
    {
        ageTimer += Time.deltaTime;

        if (ageTimer >= ageDuration)
        {
            ageTimer = 0f;

            switch (age)
            {
                case Age.Niño: age = Age.Joven; canWork = (age != Age.Niño); StatsUpdate(); SpriteSelector(); break;
                case Age.Joven: age = Age.Adulto; canWork = (age != Age.Niño); StatsUpdate(); SpriteSelector(); break;
                case Age.Adulto: age = Age.Anciano; canWork = (age != Age.Niño); StatsUpdate(); SpriteSelector(); break;
                case Age.Anciano: Die(); break;
            }
        }
    }

    public void StatsUpdate()
    {
        switch (age)
        {
            case Age.Niño: lifePoints = 10; energyPoints = 20; break;
            case Age.Joven: lifePoints = 30; energyPoints = 40; break;
            case Age.Adulto: lifePoints = 40; energyPoints = 30; break;
            case Age.Anciano: lifePoints = 20; energyPoints = 20; break;
        }
    }
}