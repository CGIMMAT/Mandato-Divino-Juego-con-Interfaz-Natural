using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolData : ItemData //Datos específicos para herramientas
{
    public int durability; //Usos antes de romperse
    public string use; //Tipo de acción
    public int effectivity; //Eficiencia en el trabajo
}