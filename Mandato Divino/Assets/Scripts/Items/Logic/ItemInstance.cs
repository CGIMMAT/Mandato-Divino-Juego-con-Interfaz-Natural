using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance //Código de parche para las instancias de los items y asó poder modificarlos
{
    public ItemData data; //Los datos de referencia del item
    public bool isEquiped; //El sistema par comprobar que un item esta equipado
    public int durability; //LA durabilidad que determina el numero e usos del item equipado

    public ItemInstance(ItemData data) //Datos que se activan para cada instancia individual
    {
        this.data = data;
        this.isEquiped = false;

        if (data is ToolData tool) durability = tool.durability; //Se extraen los datos del item en base a si es un arma o una herramieta
        else if (data is WeaponData weapon) durability = weapon.durability;
        else durability = -1;
    }

     public bool HasDurability() //Metodo para comprobar que la herramienta dad tiene stat de durabilidad y evitar errores, creada en específico para el cubo
    {
        return durability > 0;
    }

    public void Use() //Metodo para la gestión de usos de la herramienta
    {
        if (durability <= 0) return;

        durability--;

        if (durability == 0)
        {
            BreakItem();
        }
    }

    void BreakItem() //Metodo que la destruye si se agotan los usos
    {
        isEquiped = false;
        data = null;
    }
}