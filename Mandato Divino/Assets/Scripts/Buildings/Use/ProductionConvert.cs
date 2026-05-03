using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConsumptionRates //Aquí se mide lo que gasta por hora
{
    public Resources resource;
    public int amountPerHour;
}

public class ProductionConvert : ProductionBase //Sistema para las fabricas que requieren de un recurso para funcionar
{
    float timer = 0f; //Así se gestiona la producción, mediante un contador de tiempo

    public List<ConsumptionRates> costs = new List<ConsumptionRates>();

    void Update()
    {
        timer += Time.deltaTime; 

        if (timer >= 60f) //Cuando se alcanza la hora, se produce el recurso y se gasta lo indicado
        {
            Produce();
            timer = 0f;
        }
    }

    void Produce()
    {
        if (factory.needsWorker && factory.workersAssigned <= 0) return; //La función no deja actuar a las fabricas que necesitan trabajadores si aun no están asignados
        
        float totalProduction;

        if (factory.needsWorker) //Si necesita empleados, la tasa de producción se incrementa por cada trabajador
        {
            totalProduction = factory.productionRate * factory.workersAssigned;
        }
        else
        {
            totalProduction = factory.productionRate;
        }

        int amount = Mathf.FloorToInt(totalProduction); //Se redondean las cantidades a enteros para evitar problemas;
        if (amount <= 0) return; //Si la producción es nula porque aún no hay trabajadores se rompe la función

        foreach (var cost in costs) //Se comprueba que el recurso nevcesario para producir existe en cantidades adecuadas
        {
            if (RM.GetAmount(cost.resource) < cost.amountPerHour) return;
        }

        foreach (var cost in costs) //Se eliminan los recursos
        {
            RM.SpendSingle(cost.resource, cost.amountPerHour);
        }

        WorldResourceData item = RM.GetResourceData(factory.producedMaterial);
        if (item == null) return;
        DistributeItem(item, amount);
    }
}