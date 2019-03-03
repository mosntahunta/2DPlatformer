using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float initialDirection = -1.0f;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    BoxCollider2D myBoxCollider2D;
    GameObject player;
    
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        transform.localScale = new Vector2(initialDirection, 1f);

        if (player && myBoxCollider2D)
        {
            foreach (var collider in player.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(collider, myBoxCollider2D);
            }
        }
    }
    
    void Update()
    {
        Run();
    }

    private void Run()
    {
        if (isFacingRight())
        {
            myRigidBody2D.velocity = new Vector2(runSpeed, myRigidBody2D.velocity.y);
        }
        else
        {
            myRigidBody2D.velocity = new Vector2(-runSpeed, myRigidBody2D.velocity.y);
        }
        animator.SetBool("Running", true);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        float direction = Mathf.Sign(myRigidBody2D.velocity.x);
        transform.localScale = new Vector2(-direction, 1f);
    }

    bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }
}
