using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    Vector2 previousVelocity;

    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        previousVelocity = myRigidBody2D.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // get the point of contact
        ContactPoint2D contact = collision.GetContact(0);

        // reflect our old velocity off the contact point's normal vector
        Vector2 reflectedVelocity = Vector2.Reflect(previousVelocity, contact.normal);

        // assign the reflected velocity back to the rigidbody
        myRigidBody2D.velocity = reflectedVelocity;

        // rotate the object by the same ammount we changed its velocity
        Quaternion rotation = Quaternion.FromToRotation(previousVelocity, reflectedVelocity);
        transform.rotation = rotation * transform.rotation;

        print(reflectedVelocity.x);
        if (Mathf.Sign(previousVelocity.x) != Mathf.Sign(reflectedVelocity.x))
        {
            Destroy(gameObject);
        }
    }
}
