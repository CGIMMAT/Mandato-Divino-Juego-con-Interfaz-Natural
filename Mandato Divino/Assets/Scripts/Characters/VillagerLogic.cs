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

    public RuntimeAnimatorController[] maleAnims;
    public RuntimeAnimatorController[] femaleAnims;

    private SpriteRenderer SR; //Componente para cambiar el sprite del prefab
    public InventoryLogic inventory; //El inventario personal
    public ItemInstance equipedItem;

    public int attackDamage;
    public int recolectDamage;

    public Coroutine currentCoroutine;
    public FactoriesLogic currentFactory;
    public HomeLogic currentHome;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 lastValidDirection = Vector2.down;
    private Vector3 lastPosition;

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
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;

        if (inventory == null)
        {
            inventory = new InventoryLogic(inventorySlots > 0 ? inventorySlots : 5);
        }
    }
    public void Update()
    {
        GrowOlder();
        UpdateAnimationState();
    }

    public void VisualsSelector() //Función para cambiar el sprit en base al genero. Más adelante se añadirá la función para cambar el sprite en base a la edad
    {
        int ageIndex = (int)age;

        if (gender == Gender.Hombre)
        {
            SR.sprite = maleSprites[ageIndex];
            anim.runtimeAnimatorController = maleAnims[ageIndex];
        }
        else if (gender == Gender.Mujer)
        {
            SR.sprite = femaleSprites[ageIndex];
            anim.runtimeAnimatorController = femaleAnims[ageIndex];
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

        VisualsSelector();
        StatsUpdate();
        inventory = new InventoryLogic(inventorySlots); //Inicializamos su inventario personal
        audioSource = GetComponent<AudioSource>();
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
                case Age.Niño: age = Age.Joven; canWork = (age != Age.Niño); StatsUpdate(); VisualsSelector(); break;
                case Age.Joven: age = Age.Adulto; canWork = (age != Age.Niño); StatsUpdate(); VisualsSelector(); break;
                case Age.Adulto: age = Age.Anciano; canWork = (age != Age.Niño); StatsUpdate(); VisualsSelector(); break;
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
        if (anim == null || anim.runtimeAnimatorController == null) return;

        if (SR != null && !SR.enabled)
        {
            anim.SetBool("IsMoving", false);
            return;
        }

        bool moving = false;
        Vector2 direction = Vector2.zero;

        if (!isBusy)
        {
            float rbSpeed = (rb != null) ? rb.velocity.sqrMagnitude : 0f;

            if (rbSpeed > 0.0001f && rb != null)
            {
                moving = true;
                direction = rb.velocity.normalized;
            }
            
            lastPosition = transform.position;
        }
        else
        {
            Vector3 displacement = transform.position - lastPosition;
            lastPosition = transform.position;

            float calculatedSpeed = displacement.sqrMagnitude / Time.deltaTime;

            if (calculatedSpeed > 0.001f)
            {
                moving = true;
                direction = ((Vector2)displacement).normalized;
            }
        }

        if (moving)
        {
            anim.SetBool("IsMoving", true);

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                lastValidDirection = new Vector2(direction.x > 0 ? 1f : -1f, 0f);
            }
            else
            {
                lastValidDirection = new Vector2(0f, direction.y > 0 ? 1f : -1f);
            }
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }

        anim.SetFloat("MoveX", lastValidDirection.x);
        anim.SetFloat("MoveY", lastValidDirection.y);
    }
}