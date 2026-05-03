using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStandard : ProductionBase //Sistema generico en base a los datos anteriores
{
    float timer = 0f; //Así se gestiona la producción, mediante un contador de tiempo

    void Update()
    {
        timer += Time.deltaTime; 

        if (timer >= 60f) //Cuando se alcanza la hora, se produce el recurso
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

        WorldResourceData item = RM.GetResourceData(factory.producedMaterial);

        if (item == null) return;
        DistributeItem(item, amount);
    }
}