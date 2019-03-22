using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    Vector2 previousVelocity;
    Animator animator;

    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (myRigidBody2D.velocity.x == 0.0f)
        {
            Impact();
        }
        previousVelocity = myRigidBody2D.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Friend") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            Destroy(collision.gameObject);
            Impact();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground") ||
                 collision.gameObject.layer == LayerMask.NameToLayer("Brick"))
        {
            // get the point of contact
            ContactPoint2D contact = collision.GetContact(0);

            // reflect our old velocity off the contact point's normal vector
            Vector2 reflectedVelocity = Vector2.Reflect(previousVelocity, contact.normal);

            // assign the reflected velocity back to the rigidbody
            myRigidBody2D.velocity = reflectedVelocity;

            // rotate the object by the same amount we changed its velocity
            Quaternion rotation = Quaternion.FromToRotation(previousVelocity, reflectedVelocity);
            transform.rotation = rotation * transform.rotation;

            if (Mathf.Sign(previousVelocity.x) != Mathf.Sign(reflectedVelocity.x))
            {
                Impact();
            }
        }
    }

    private void Impact()
    {
        myRigidBody2D.velocity = new Vector2(0, 0);
        myRigidBody2D.gravityScale = 0;
        animator.SetBool("Collided", true);
        Destroy(gameObject, 0.1f);
    }
}
