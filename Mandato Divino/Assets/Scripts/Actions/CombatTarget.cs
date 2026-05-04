using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CombatTarget
{
    void TakeDamage(int amount);
    Transform GetTransform();
    bool isDead();
}
