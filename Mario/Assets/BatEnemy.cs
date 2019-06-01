using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : HittableObject
{
    protected override void OnHit(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
