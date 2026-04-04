using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerLogic : MonoBehaviour //Datos que deberá tener el prefab del aldeano cuando se genere
{
    public string villagerName; //Su nombre
    public int id; //Su identficador

    public Age age; //Su edad
    public Gender gender; //Su genero, ambos se usaran para determianr su sprite

    public int lifePoints; //Su vida
    public int energyPoints; //Las acciones que puede hacer antes de descansar
    public int inventorySlots; //Los items que puede llevar

    public bool canWork; //Si puede trabajar
    public Relationship relationship; //Su relación y estado amoroso

    public Sprite maleSprite; //Sprite cuando es hombre
    public Sprite femaleSprite; //Sprite cuando es mujer

    private SpriteRenderer SR; //Componente para cambiar el sprite del prefab
    public InventoryLogic inventory; //El inventario personal

    void Awake()
    {
        SR = GetComponent<SpriteRenderer>(); //Inizializamos el componente del sprite para poder manipularlo con el siguiente metodo
    }

    public void SpriteSelector() //Función para cambiar el sprit en base al genero. Más adelante se añadirá la función para cambar el sprite en base a la edad
    {
        if (gender == Gender.Male)
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

        age = newAge;
        gender = (Gender) Random.Range(0,2); //Se asigna aleatoriamnete el genero de personaje
        villagerName = NameManager.Instance.GetRandomName(gender);
        canWork = (age != Age.Child); //Identifica si el aldeano puede trabajar

        relationship = new Relationship //Una relación inicial para marcar que el aldeano aún no tiene pareja
        {
            LoverID = 0,
            inLove = false
        };

        SpriteSelector();
        inventory = new InventoryLogic(inventorySlots); //Inicializamos su inventario personal
    }
}