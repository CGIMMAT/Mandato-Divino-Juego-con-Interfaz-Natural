using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buildings/Factory")]
public class FactoriesData : BuildingData //Datos especificos para crear fabricar o generadores de recursos
{
    public Resources producedMaterial; //LO que se produce
    public float productionRate; //La cantidad por hora del juego que se produce
    public int maxStorage; //Capacidad maxima de recursos que se pueden guardar aquí si no hay almacenes
    public int workBenches; //Maximos huecos para trabajadores, en casi todos es necesario minimo uno para empezar a producir
    public bool needsWorker; //Diferencia los edificios que necesitan un trabajador para producir de los que van en automatico
}
//La tasa de producción se mide en automático o en base a un trabajador, meter más trabajadores la aumeta