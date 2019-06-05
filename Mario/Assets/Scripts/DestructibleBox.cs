using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBox : HittableObject
{
    protected override void Start()
    {
        base.Start();
        GetComponent<BoxCollider2D>().edgeRadius = 0.03f;
    }

    protected override void OnHit(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
