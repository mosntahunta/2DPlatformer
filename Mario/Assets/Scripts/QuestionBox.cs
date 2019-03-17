using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    Animator animator;
    GameObject player;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == player.gameObject.tag)
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 center = collision.collider.bounds.center;

            float minX = collision.collider.bounds.min.x;
            float maxX = collision.collider.bounds.max.x;

            if (contactPoint.y > center.y && contactPoint.x > minX && contactPoint.x < maxX)
            {
                animator.SetBool("Dying", true);
            }
        }
    }
}
