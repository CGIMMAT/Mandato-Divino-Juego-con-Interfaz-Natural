using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainResourceLogic : MonoBehaviour //Código para los prefabs de las fuentes
{
    public string resourceName;
    public Resources resourceType;

    public int life;
    public int maxLife; //Se usa como referencia para que se sepa cual es la vida máxima que puede recuperar una vez recuperado
    public int resourcesPeLife;
    public int recoveryTime;

    protected FountainResourceData data;
    protected bool isRecovering = false;
    private SpriteRenderer spriteRenderer;
    private Collider colliderComponent;

    public virtual void Initialize(FountainResourceData resourceData)
    {
        data = resourceData;

        resourceName = data.resourceName;
        resourceType = data.resourceType;

        maxLife = data.life;
        life = data.life;

        resourcesPeLife = data.resourcesPeLife;
        recoveryTime = data.recoveryTime;

        spriteRenderer = GetComponent<SpriteRenderer>();
        colliderComponent = GetComponent<Collider>();
    }

    public IEnumerator Recover() //La corrutina para que se recupere o no en base al booleano correspondiente
    {
        isRecovering = true;
        SetActiveState(false);
        yield return new WaitForSeconds(recoveryTime * 60);

        life = maxLife;
        SetActiveState(true);
        isRecovering = false;
    }

    public void SetActiveState(bool state) //Se usa como control para determinar si es momento de recuperar o no
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = state;

        Collider col = GetComponent<Collider>();
        if (col != null) 
        {
            col.enabled = state;
        }
        else 
        {
            Collider2D col2D = GetComponent<Collider2D>();
            if (col2D != null) col2D.enabled = state;
        }
    }

    public void StartRecovery()
    {
        if (!isRecovering)
        {
            StartCoroutine(Recover());
        }
    }
}
