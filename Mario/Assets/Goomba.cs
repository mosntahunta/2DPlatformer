using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float direction = -1.0f;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    Vector2 scaleVector;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        scaleVector = new Vector2(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        Run();
    }

    private void Run()
    {
        myRigidBody2D.velocity = new Vector2(direction * runSpeed, myRigidBody2D.velocity.y);
        animator.SetBool("Running", true);
        FlipSprite(direction);
    }

    private void FlipSprite(float direction)
    {
        scaleVector.x = direction;
        scaleVector.y = 1f;

        transform.localScale = scaleVector;
    }
}
