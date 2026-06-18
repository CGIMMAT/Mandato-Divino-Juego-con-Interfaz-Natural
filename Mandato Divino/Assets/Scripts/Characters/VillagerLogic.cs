using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerLogic : MonoBehaviour, CombatTarget //Datos que deberá tener el prefab del aldeano cuando se genere
{
    private VillagersData Data;
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

    private Animator anim;
    private Vector3 lastPosition;
    private Vector2 lastValidDirection = Vector2.down; 
    private Rigidbody2D rb;

    private float ageTimer; //EL contador de tiempo que mide cuanto está vivo
    private const float ageDuration = 4320f; //3 días medidos en segundos, el tiempo que se tarda en envejecer

    private AudioSource audioSource; //Componente para que se ejecuten sonidos al realizar acciones

    public enum VillagerSounds
    {
        Silence,
        Recolect,
        Combat,
        Cook
    }

    private VillagerSounds currentSound = VillagerSounds.Silence;

    void Awake()
    {
        SR = GetComponent<SpriteRenderer>(); //Inizializamos el componente del sprite para poder manipularlo con el siguiente metodo
        anim = GetComponent<Animator>();
        lastPosition = transform.position;

        if (inventory == null)
        {
            inventory = new InventoryLogic(inventorySlots > 0 ? inventorySlots : 5);
        }
    }
    public void Update()
    {
        GrowOlder();
    }

    public void FixedUpdate()
    {
        UpdateAnimationState();
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
        rb = GetComponent<Rigidbody2D>();

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
        audioSource = GetComponent<AudioSource>();

        SyncAnimatorIdentity();
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
                case Age.Niño: age = Age.Joven; canWork = (age != Age.Niño); StatsUpdate(); SpriteSelector(); SyncAnimatorIdentity(); break;
                case Age.Joven: age = Age.Adulto; canWork = (age != Age.Niño); StatsUpdate(); SpriteSelector(); SyncAnimatorIdentity(); break;
                case Age.Adulto: age = Age.Anciano; canWork = (age != Age.Niño); StatsUpdate(); SpriteSelector(); SyncAnimatorIdentity(); break;
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

    public void PlaySound(VillagerSounds newSound)
    {
        if (currentSound == newSound) return;

        currentSound = newSound;
        audioSource.Stop();

        switch (newSound)
        {
            case VillagerSounds.Recolect: audioSource.clip = Data.recolectSound; break;
            case VillagerSounds.Combat: audioSource.clip = Data.combatSound; break;
            case VillagerSounds.Cook: audioSource.clip = Data.cookSound; break;
            case VillagerSounds.Silence: audioSource.clip = null; return;
        }

        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void UpdateAnimationState()
    {
        if (anim == null || rb == null) return;

        // Si el SpriteRenderer está apagado, forzar parada
        if (SR != null && !SR.enabled)
        {
            anim.SetBool("IsMoving", false);
            return;
        }

        Vector2 currentVelocity = rb.velocity;

        // Umbral físico para detectar movimiento real
        if (currentVelocity.sqrMagnitude > 0.005f)
        {
            anim.SetBool("IsMoving", true);
            Vector2 direction = currentVelocity.normalized;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                lastValidDirection = new Vector2(direction.x > 0 ? 1f : -1f, 0f);
            }
            else
            {
                lastValidDirection = new Vector2(0f, direction.y > 0 ? 1f : -1f);
            }

            anim.SetFloat("MoveX", lastValidDirection.x);
            anim.SetFloat("MoveY", lastValidDirection.y);
        }
        else
        {
            anim.SetBool("IsMoving", false);
            anim.SetFloat("MoveX", lastValidDirection.x);
            anim.SetFloat("MoveY", lastValidDirection.y);
        }
    }

    public void SyncAnimatorIdentity()
    {
        if (anim == null) return;

        bool hasAgeState = false;
        bool hasGenderState = false;

        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == "AgeState") hasAgeState = true;
            if (param.name == "GenderState") hasGenderState = true;
        }

        if (!hasAgeState || !hasGenderState) return;

        int genderValue = (gender == Gender.Hombre) ? 0 : 1;
        anim.SetInteger("GenderState", genderValue);

        int ageValue = 0;
        switch (age)
        {
            case Age.Niño: ageValue = 0; break;
            case Age.Joven: ageValue = 1; break;
            case Age.Adulto: ageValue = 2; break;
            case Age.Anciano: ageValue = 3; break;
        }
        anim.SetInteger("AgeState", ageValue);
    }
}