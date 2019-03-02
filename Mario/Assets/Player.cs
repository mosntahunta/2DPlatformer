using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float acceleration = 10.0f;
    [SerializeField] float jumpSpeed = 10.0f;
    [SerializeField] float drag = 2.5f;
    [SerializeField] float epsilon = 0.2f;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    Vector2 scaleVector;
    BoxCollider2D boxColliderCache;

    private bool isFalling = false;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        scaleVector = new Vector2(0f, 0f);
        boxColliderCache = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        if (myRigidBody2D.velocity.y < -0.1)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
        
        Run();
    }

    private void Run()
    {
        bool change_direction = false;

        // limit speed
        if (Mathf.Abs(myRigidBody2D.velocity.x) > runSpeed)
        {
            myRigidBody2D.velocity = new Vector2(runSpeed * Mathf.Sign(myRigidBody2D.velocity.x), myRigidBody2D.velocity.y); ;
        }

        // input
        float control_throw = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 to 1
        bool apply_movement = Mathf.Abs(control_throw) > 0.15f;

        if (apply_movement)
        {
            float direction = Mathf.Sign(control_throw);

            if (direction != Mathf.Sign(myRigidBody2D.velocity.x))
            {
                change_direction = true;
                myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x / 2, myRigidBody2D.velocity.y);
            }

            FlipSprite(direction);
        }

        // apply force or drag
        if (apply_movement)
        {
            myRigidBody2D.drag = 0;
            float horizontal_force = control_throw * acceleration;
            Vector2 force = new Vector2(horizontal_force, 0.0f);
            myRigidBody2D.AddForce(force);
        }
        else
        {
            myRigidBody2D.drag = drag;
        }

        // animation
        if (TouchingGround())
        {
            if (change_direction)
            {
                animator.SetBool("Turning", apply_movement);
            }
            else
            {
                bool player_has_hspeed = Mathf.Abs(myRigidBody2D.velocity.x) > epsilon || control_throw != 0.0f;
                animator.SetBool("Turning", false);
                animator.SetBool("Running", player_has_hspeed);
                animator.speed = Mathf.Max(0.5f, 2 * Mathf.Abs(myRigidBody2D.velocity.x) / runSpeed);
            }
        }
    }

    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            animator.SetBool("Jumping", true);

            if (TouchingGround())
            {
                Vector2 jump_velocity = new Vector2(myRigidBody2D.velocity.x, jumpSpeed);
                myRigidBody2D.velocity = jump_velocity;
            }
        }

        if (isFalling && TouchingGround())
        {
            animator.SetBool("Jumping", false);
        }
    }

    private void FlipSprite( float direction )
    {
        scaleVector.x = direction;
        scaleVector.y = 1f;

        transform.localScale = scaleVector;
    }

    private bool TouchingGround()
    {
        return boxColliderCache.IsTouchingLayers(LayerMask.GetMask("Foreground"));
    }
}
