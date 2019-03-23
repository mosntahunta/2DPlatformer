using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaTroopa : MonoBehaviour
{
    [SerializeField] float shellSpeed = 10.0f;

    Rigidbody2D myRigidBody2D;
    Animator animator;
    GameObject player;
    Patrol patrol;

    public State state;

    public enum State
    {
        IDLE,
        PATROL,
        SHELL_IDLE,
        SHELL_PATROL
    }

    IEnumerator IdleState()
    {
        if (state == State.IDLE)
        {
            yield return 0;
        }
        NextState();
    }

    IEnumerator PatrolState()
    {
        if (state == State.PATROL)
        {
            animator.SetBool("Running", true);
            patrol.Proceed();
            yield return 0;
        }
        NextState();
    }

    IEnumerator ShellIdleState()
    {
        if (state == State.SHELL_IDLE)
        {
            animator.SetBool("Shell", true);
            yield return 0;
        }
        NextState();
    }

    IEnumerator ShellPatrolState()
    {
        if (state == State.SHELL_PATROL)
        {
            animator.SetBool("Shell", true);
            patrol.Proceed();
            yield return 0;
        }
        NextState();
    }

    void NextState()
    {
        switch (state)
        {
            case State.IDLE:
                StartCoroutine("IdleState");
                break;
            case State.PATROL:
                StartCoroutine("PatrolState");
                break;
            case State.SHELL_IDLE:
                StartCoroutine("ShellIdleState");
                break;
            case State.SHELL_PATROL:
                StartCoroutine("ShellPatrolState");
                break;
            default:
                break;
        }
    }

    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        patrol = GetComponent<Patrol>();

        // by default koopa patrols the ground
        state = State.PATROL;
    }

    void Update()
    {
        NextState();
    }

    // handle death when colliding with the player's feet
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == player.gameObject.tag)
        {
            if (state != State.SHELL_IDLE)
            {
                Collider2D collider = collision.collider;
                if (collider.GetType() == typeof(BoxCollider2D))
                {
                    Vector3 contactPoint = collision.GetContact(0).point;
                    Vector3 center = collider.bounds.center;

                    if (contactPoint.y <= center.y)
                    {
                        player.GetComponent<Player>().PushUp();

                        myRigidBody2D.velocity = new Vector2(0.0f, myRigidBody2D.velocity.y);

                        state = State.SHELL_IDLE;
                    }
                }
            }
            else if (state == State.SHELL_IDLE)
            {
                Collider2D collider = collision.otherCollider;

                Vector3 contactPoint = collision.GetContact(0).point;
                Vector3 center = collider.bounds.center;

                if (contactPoint.x < center.x)
                {
                    patrol.SetVelocity(1.0f, shellSpeed);
                }
                else
                {
                    patrol.SetVelocity(-1.0f, shellSpeed);
                }

                state = State.SHELL_PATROL;
            }
        }
        else if (state == State.SHELL_PATROL && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

    // layer changes must be on collision exit, otherwise there can be 
    // unexpected collision with the player
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == player.gameObject.tag )
        {
            if (state == State.SHELL_PATROL)
            {
                gameObject.layer = LayerMask.NameToLayer("Neutral");
            }
            else if (state == State.SHELL_IDLE)
            {
                gameObject.layer = LayerMask.NameToLayer("Friend");
            }
            
        }
    }
}
