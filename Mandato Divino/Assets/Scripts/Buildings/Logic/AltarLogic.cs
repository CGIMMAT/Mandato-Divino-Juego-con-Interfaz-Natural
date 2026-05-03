using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarLogic : BuildingsLogic //Para que el prefab del altar inicialice los dates correspondientes
{
    public int areaSize;

    public void Initialize(AltarData data)
    {
        base.Initialize(data);
        areaSize = data.areaSize;
    }
}