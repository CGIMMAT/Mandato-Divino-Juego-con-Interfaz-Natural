using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Altar")]
public class AltarData : BuildingData //Datos específicos para crear altares
{
    public int areaSize; //Como son edificios especiales en torno a los cuales se debe construir el pueblo cuentan con un area que se puede ampliar con más edificios
}