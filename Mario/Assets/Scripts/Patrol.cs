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
    public float horizontalSpeed = 5.0f;
    public float direction = -1.0f;
    public BoxCollider2D forwardBoxCollider;

    Rigidbody2D myRigidBody2D;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        // set the initial direction of the game object
        transform.localScale = new Vector2(direction, 1f);

        // ignore the obstacle detection for the player game object
        if (player && forwardBoxCollider)
        {
            foreach (var collider in player.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(collider, forwardBoxCollider);
            }
        }
    }

    public void SetVelocity(float direction, float horizontalSpeed)
    {
        this.direction = direction;
        this.horizontalSpeed = horizontalSpeed;
        transform.localScale = new Vector2(direction, 1f);
    }

    // Begin applying velocity to the game object in a direction it is facing
    public void Proceed()
    {
        myRigidBody2D.velocity = new Vector2(direction * horizontalSpeed, myRigidBody2D.velocity.y);
    }
    
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            // change direction
            direction = -direction;
            transform.localScale = new Vector2(direction, 1f);
        }
    }
}
