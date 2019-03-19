using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickDestroy : MonoBehaviour
{
    public Vector2 initialVelocity;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.velocity = initialVelocity;
    }
}
