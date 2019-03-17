using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    // State
    bool isAlive = true;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    CapsuleCollider2D myCapsuleCollider2D;
    GameObject player;
    Patrol patrol;
    
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        patrol = GetComponent<Patrol>();
    }
    
    void Update()
    {
        if (!isAlive) { return; }

        animator.SetBool("Running", true);
        patrol.Proceed();
    }

    // handle death when colliding with the player's feet
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == player.gameObject.tag)
        {
            Collider2D player_collider = collision.collider;

            if (player_collider.GetType() == typeof(BoxCollider2D))
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                Vector2 center = player_collider.bounds.center;

                if (contactPoint.y < center.y)
                {
                    isAlive = false;
                    Rigidbody2D player_rigidbody = player.GetComponent<Rigidbody2D>();
                    player_rigidbody.velocity = new Vector2(player_rigidbody.velocity.x, 15.0f);
                    myRigidBody2D.velocity = new Vector2(0.0f, 0.0f);
                    animator.SetTrigger("Dying");
                    foreach (var collider in player.GetComponentsInChildren<Collider2D>())
                    {
                        Physics2D.IgnoreCollision(collider, myCapsuleCollider2D);
                    }
                    Destroy(gameObject, 1);
                }
            }
        }
    }
}
