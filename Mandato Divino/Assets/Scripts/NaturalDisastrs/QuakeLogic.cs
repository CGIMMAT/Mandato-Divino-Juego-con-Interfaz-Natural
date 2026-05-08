using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakeLogic : MonoBehaviour
{
    public void Initialize(float intensity)
    {
        StartCoroutine(Quake(intensity));
    }

    IEnumerator Quake(float intensity)
    {
        int shakes = Mathf.RoundToInt(2 * intensity);
        for (int i = 0; i < shakes; i++)
        {
            ApplyAreaDamage();
            yield return new WaitForSeconds(2f);
        }

        Destroy(gameObject);
    }

    public void ApplyAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(3,3), 0);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out BuildingsLogic building))
            {
                building.TakeDamage(10);
            }
            else if (hit.TryGetComponent(out VillagerLogic villager))
            {
                villager.TakeDamage(2);
            }
        }
    }
}
