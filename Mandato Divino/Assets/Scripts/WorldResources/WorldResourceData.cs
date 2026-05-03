using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/WorldResource")] 
public class WorldResourceData : BaseItemsData //Datos de los recursos del jugador
{
    public string resourceName; //Nombre del recurso
    public GameObject resourcePrefab; //Su imagen
    public int value; //Cuanto valor aumenta en el contador al recoger el recurso

    private void OnEnable() //Al activarse cualquier objeto de tipo WorldResourceDaata
    {
        isStackable = true; //Se define como stackeable
        maxStack = 10; //Y que su maximo stack es 10
    }
}