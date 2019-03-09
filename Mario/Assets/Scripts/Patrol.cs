using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// Implements the patrolling ground movement of an enemy.
/// 
/// Automatically change the direction of an enemy gameobject
/// when it is approaching a wall or a ledge with an obstacle detection collider.
/// 
/// You must implement a forward box collider to detect
/// changes in trigger collision with other colliders infront 
/// of the gameobject.
/// 
/// Can set the game object's initial facing direction and its patrol speed.
/// 
public class Patrol : MonoBehaviour
{
    [SerializeField] float horizontal_speed = 5.0f;
    [SerializeField] float initialDirection = -1.0f;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    BoxCollider2D myBoxCollider2D;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        // set the initial direction of the game object
        transform.localScale = new Vector2(initialDirection, 1f);

        // ignore the obstacle detection for the player game object
        if (player && myBoxCollider2D)
        {
            foreach (var collider in player.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(collider, myBoxCollider2D);
            }
        }
    }

    // Begin applying velocity to the game object in a direction it is facing
    public void Proceed()
    {
        if (isFacingRight())
        {
            myRigidBody2D.velocity = new Vector2(horizontal_speed, myRigidBody2D.velocity.y);
        }
        else
        {
            myRigidBody2D.velocity = new Vector2(-horizontal_speed, myRigidBody2D.velocity.y);
        }
        animator.SetBool("Running", true);
    }

    bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        float direction = Mathf.Sign(myRigidBody2D.velocity.x);
        transform.localScale = new Vector2(-direction, 1f);
    }
}
