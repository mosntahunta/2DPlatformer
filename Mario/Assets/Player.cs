using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField] float runSpeed = 5.0f;

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
        FlipSprite();
    }

    private void Run()
    {
        float control_throw = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 to 1
        float horizontal_velocity = control_throw * runSpeed;
        Vector2 player_velocity = new Vector2(horizontal_velocity, myRigidBody2D.velocity.y);
        myRigidBody2D.velocity = player_velocity;

        bool player_has_hspeed = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;
        animator.SetBool("Running", player_has_hspeed);
    }

    private void FlipSprite()
    {
        // if the player is moving horizontally, reverse the scaling of the x-axis

        bool player_has_hspeed = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;

        if (player_has_hspeed)
        {
            float direction = Mathf.Sign(myRigidBody2D.velocity.x);

            scaleVector.x = direction;
            scaleVector.y = 1f;

            transform.localScale = scaleVector;
        }
    }
}
