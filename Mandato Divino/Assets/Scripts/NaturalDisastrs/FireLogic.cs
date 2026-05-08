using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLogic : MonoBehaviour
{
    private Vector3Int tilePos;
    private ResourceGenerator RG;
    private float intensity;

    public void Initialize(Vector3Int pos, float intensity, ResourceGenerator RG)
    {
        this.tilePos = pos;
        this.intensity = intensity;
        this.RG = RG;
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        GameObject resource = RG.GetResourceAt(tilePos);
        if (resource == null) yield break;

        FountainResourceLogic fountain = resource.GetComponent<FountainResourceLogic>();
        fountain.SetActiveState(false);

        float duration = 5f * intensity;
        float timer = 0f;

        while (timer < duration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out VillagerLogic villager))
                {
                    villager.TakeDamage(Mathf.RoundToInt(2f * intensity));
                }
            }
            timer += 1f;
            yield return new WaitForSeconds(1f);
        }

        fountain.life = 0;
        fountain.StartCoroutine(fountain.Recover());
        Destroy(gameObject);
    }
}
