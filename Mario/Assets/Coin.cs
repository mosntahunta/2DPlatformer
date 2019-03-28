using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool canBePickedUp = true;

    public void KickUp(Vector2 upVelocity)
    {
        gameObject.layer = LayerMask.NameToLayer("Transparent");
        canBePickedUp = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = upVelocity;

        Destroy(gameObject, 0.45f);
    }

    public bool CanBePickedUp()
    {
        return canBePickedUp;
    }
}
