using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : HittableObject
{
    public float patrolDistanceY = 10f;
    public float directionY = -1f;

    public float patrolWaitTime = 1f;
    private float patrolWaitTimer = 0f;

    public float maxSpeed = 10f;
    public float minSpeed = 2f;
    private float verticalSpeed = 0f;

    private float startY = 0f;

    // state references
    private int idleState = 0;
    private int patrolState = 1;

    private StateMachine stateMachine;

    protected override void Start()
    {
        startY = rb2d.position.y;
        patrolWaitTimer = patrolWaitTime;

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.Initialise(2);
        stateMachine.SetCallbacks(idleState, null, null, IdleUpdate, null);
        stateMachine.SetCallbacks(patrolState, null, null, PatrolUpdate, null);
        stateMachine.SetState(idleState);
    }

    int IdleUpdate()
    {
        verticalSpeed = 0f;

        if (patrolWaitTimer < 0)
        {
            directionY *= -1f;
            startY = rb2d.position.y;
            patrolWaitTimer = patrolWaitTime;

            return patrolState;
        }
        else
        {
            patrolWaitTimer -= Time.deltaTime;
        }

        return idleState;
    }

    int PatrolUpdate()
    {
        float midPointY = patrolDistanceY / 2;
        float distanceToMidPoint = Mathf.Abs(startY - rb2d.position.y) - midPointY;

        verticalSpeed = Mathf.Lerp(maxSpeed, minSpeed, Mathf.Abs(distanceToMidPoint) / midPointY) * directionY;

        if (Mathf.Abs(startY - rb2d.position.y) > patrolDistanceY)
        {
            return idleState;
        }

        return patrolState;
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(0f, verticalSpeed);
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
