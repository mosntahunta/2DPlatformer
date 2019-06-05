using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : PhysicsObject
{
    public float maxHealth;

    protected float currentHealth;

    protected override void Start()
    {
        base.Start();
        currentHealth = maxHealth;
    }

    // the inherited classes will determine how they will process the damage
    protected virtual void OnHit(float damage)
    {
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            Attack attack = collider.gameObject.GetComponent<Attack>();
            OnHit(attack.damage);
        }
    }

}
