using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : HittableObject
{
    public float facingDirection = 1.0f;

    public float maxHorizontalSpeed = 3f;
    public float jumpSpeed = 5f;

    public float jumpTime = 1f;
    private float jumpTimer = 0;

    // state references
    private int idleState = 0;
    private int jumpState = 1;

    private StateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.Initialise(2);
        stateMachine.SetCallbacks(idleState, null, null, IdleUpdate, null);
        stateMachine.SetCallbacks(jumpState, null, null, JumpUpdate, null);
        stateMachine.SetState(idleState);

        jumpTimer = jumpTime;

        GetComponent<BoxCollider2D>().edgeRadius = 0.03f;
    }

    int IdleUpdate()
    {
        if (jumpTimer < 0)
        {
            jumpTimer = jumpTime;
            velocity.y = jumpSpeed;
            grounded = false;
            return jumpState;
        }
        else
        {
            jumpTimer -= Time.deltaTime;
        }

        return idleState;
    }

    int JumpUpdate()
    {
        // change direction if a wall is detected
        // todo: is there an easy way to get all the layer mask for the current game object?
        RaycastHit2D hit = Physics2D.Raycast(rb2d.position, new Vector2(facingDirection, 0), 0.5f, LayerMask.GetMask("Ground", "Platform", "Neutral"));
        if (hit.collider != null)
        {
            facingDirection *= -1;
        }

        targetVelocity.x = facingDirection * maxHorizontalSpeed;

        if (grounded)
        {
            targetVelocity.x = 0f;
            return idleState;
        }

        return jumpState;
    }

    void Update()
    {
        ApplyGravity();
    }

    protected override void OnCollision(Collider2D collider, Vector2 contactPosition)
    {
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
