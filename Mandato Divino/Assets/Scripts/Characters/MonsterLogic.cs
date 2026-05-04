using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLogic : MonoBehaviour, CombatTarget
{
    public string monsterName; //Su nombre
    public int lifePoints; //Su vida
    public int attackDamage; //El daño que hacen al atacar
    public int lifeSpan; //El tiempo que vive

    private Transform target; //El objetivo de sus ataques
    private Vector3 wanderTarget; //El objetivo hacia el cual se mueven
    private float wanderTimer;
    private float attackCooldown;

    public void Initialize(MonsterData data)
    {
        monsterName = data.enemyName;
        lifePoints = data.lifePoints;
        attackDamage = data.attackDamage;
        lifeSpan = Random.Range(data.lifeSpan, (data.lifeSpan * 5) + 1); //Se aleatoriza la cantidad de tiempo que viven

        SetNewTarget();
    }

    public void Update()
    {
        if (isDead()) return;

        FindTarget();

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= 2f)
            {
                AttackTarget();
            }
            else if (dist <= 5f)
            {
                MoveToTarget(target.position);
            }
            else
            {
                WanderToTarget(target.position);
            }
        }
        else
        {
            Wander();
        }
    }

    public void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5f);
        float bestDist = Mathf.Infinity;
        foreach (var hit in hits)
        {
            VillagerLogic villager = hit.GetComponent<VillagerLogic>();
            BuildingsLogic building = hit.GetComponent<BuildingsLogic>();

            if (villager != null && !villager.isBusy || building != null)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    target = hit.transform;
                }
            }
        }
    }

    public void MoveToTarget(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 2f);
    }

    public void WanderToTarget(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        Vector3 noise = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        transform.position += (dir + noise * 0.5f).normalized * Time.deltaTime * 1.6f;
    }

    public void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f) SetNewTarget();
        MoveToTarget(wanderTarget);
    }

    public void SetNewTarget()
    {
        wanderTarget = transform.position + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);
        wanderTimer = Random.Range(3f, 8f);
    }

    public void AttackTarget()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            CombatTarget CT = target.GetComponent<CombatTarget>();
            if (CT != null) CT.TakeDamage(attackDamage);
            attackCooldown = 2f;
        }
    }

    public void TakeDamage(int amount)
    {
        lifePoints -= amount;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool isDead()
    {
        return lifePoints <= 0;
    }
}