using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormLogic : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed = 2f;

    public void Initialize(float intensity)
    {
        moveDirection = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), 0).normalized;
        float duration = 10f * intensity;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(2,2), 0);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out VillagerLogic villager))
            {
                villager.TakeDamage(2);
            }
            else if (hit.TryGetComponent(out BuildingsLogic building))
            {
                building.TakeDamage(1);
            }
        }
    }
}
