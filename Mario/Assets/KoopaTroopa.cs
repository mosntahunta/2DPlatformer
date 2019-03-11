using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaTroopa : MonoBehaviour
{
    // State
    bool shell = false;

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
        if (shell) { return; }

        patrol.Proceed();
    }

    // handle death when colliding with the player's feet
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!shell)
            {
                Collider2D collider = collision.collider;

                Vector3 contactPoint = collision.GetContact(0).point;
                Vector3 center = collider.bounds.center;

                if (contactPoint.y <= center.y)
                {
                    Rigidbody2D player_rigidbody = player.GetComponent<Rigidbody2D>();
                    player_rigidbody.velocity = new Vector2(player_rigidbody.velocity.x, 15.0f);
                    myRigidBody2D.velocity = new Vector2(0.0f, myRigidBody2D.velocity.y);
                    animator.SetBool("Shell", true);
                    shell = true;
                }
            }
        }
    }
}
