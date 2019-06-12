using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : MonoBehaviour
{
    public float patrolTime = 3f;
    private float patrolTimer = 0f;

    public float horizontalVelocity = 3f;
    public float verticalVelocity = 3f;

    private Vector2 targetVelocity;

    private Rigidbody2D rb2d;

    void OnEnable()
    {
        patrolTimer = patrolTime;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolTimer > 0)
        {
            patrolTimer -= Time.deltaTime;
            targetVelocity.x = horizontalVelocity;
            targetVelocity.y = verticalVelocity;
        }
        else
        {
            horizontalVelocity = -horizontalVelocity;
            verticalVelocity = -verticalVelocity;
            patrolTimer = patrolTime;
        }
    }

    void FixedUpdate()
    {
        Vector2 deltaPosition = targetVelocity * Time.deltaTime;
        rb2d.position = rb2d.position + deltaPosition;
    }
}
