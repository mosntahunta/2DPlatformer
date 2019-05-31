using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    protected int health;

    protected virtual void OnHit()
    {
        Debug.Log("hit");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Todo: this should be more generic as there could be other ways of hitting such as melee attacks, bombs, etc
        if (collider.tag == "PlayerBullet")
        {
            OnHit();
        }
    }

}
