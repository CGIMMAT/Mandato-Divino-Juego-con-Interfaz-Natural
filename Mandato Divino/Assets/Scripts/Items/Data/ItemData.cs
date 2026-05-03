using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID
{
    Hacha,
    HachaMetal,
    Mochila,
    Cubo,
    Martillo,
    MartilloMetal,
    Sarten,
    Pico,
    PicoMetal,
    Pala,
    Balista,
    Arco,
    FlechaMadera,
    FlechaPiedra,
    FlechaMetal,
    ProyectilMadera,
    ProyectilPiedra,
    ProyectilMetal,
    EscudoMadera,
    EscudoPiedra,
    EscudoMetal,
    EspadaMadera,
    EspadaPiedra,
    EspadaMetal,
    LanzaMadera,
    LanzaPiedra,
    LanzaMetal,
    MazaMadera,
    MazaPiedra,
    MazaMetal,
}

public enum ActionType
{
    None,
    Talar,
    Cargar,
    Construir,
    Cocinar,
    Minar,
    Cavar,
    Artilleria,
    Lanzar,
    Disparar,
    Proyectil,
    Proteger,
    Clavar,
    Rajar,
    Aplastar,
}

public abstract class ItemData : BaseItemsData //Datos genericos para todos los items que no son recursos
{
    public GameObject itemPrefab; //El objeto en sí instanciable
    public ItemID id; //Su identificador
    public ActionType actionType; //El tipo de acción que realiza el items
    public List<Cost> productionCost; //Lo que cuestan
    public float productionTime; //Lo que se tarda
    public bool isEquipable; //SI se puede equipar
    public bool isEquiped;

    private void OnEnable() //Al activarse cualquier objeto de tipo ItemData
    {
        isStackable = false; //Se define como no stackeable
        maxStack = 1; //Y por tanto no tiene stacks, solo se guarda de uno en uno
    }
}