using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    private BoxCollider2D meleeHitbox;

    void Start()
    {
        meleeHitbox = GetComponent<BoxCollider2D>();
        meleeHitbox.enabled = false; // weapon is disabled by default
    }

    public void Sheathe()
    {
        meleeHitbox.enabled = false;
    }

    public void Unsheathe()
    {
        meleeHitbox.enabled = true;
    }
}
