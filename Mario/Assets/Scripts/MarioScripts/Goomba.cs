using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public int deathScore = 100;
    public BoxCollider2D hitBox;

    // State
    bool isAlive = true;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    GameObject player;
    Patrol patrol;
    
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        patrol = GetComponent<Patrol>();

        hitBox.edgeRadius = 0.01f;
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
                    player.GetComponent<Player>().PushUp();

                    GameControl.control.setScore(GameControl.control.getScore() + deathScore);

                    PointDisplay pointDisplay = GameObject.FindGameObjectWithTag("PointDisplay").GetComponent<PointDisplay>();
                    if (pointDisplay)
                    {
                        pointDisplay.DisplayScoreAtPosition(deathScore, transform.position);
                    }
                    
                    Death();
                }
            }
        }
    }

    void Death()
    {
        myRigidBody2D.velocity = new Vector2(0.0f, 0.0f);
        gameObject.layer = LayerMask.NameToLayer("EnemyDeath");
        animator.SetTrigger("Dying");
        Destroy(gameObject, 1);
    }
}
