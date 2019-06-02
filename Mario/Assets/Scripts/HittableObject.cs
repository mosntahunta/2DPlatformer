using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    public float maxHealth;

    protected float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // the inherited classes will determine how they will process the damage
    protected virtual void OnHit(float damage)
    {
        Debug.Log("hit");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Todo: this should be more generic as there could be other ways of hitting such as melee attacks, bombs, etc
        if (collider.tag == "PlayerBullet")
        {
            BasicProjectile projectile = collider.gameObject.GetComponent<BasicProjectile>();
            OnHit(projectile.damage);
        }
    }

}
