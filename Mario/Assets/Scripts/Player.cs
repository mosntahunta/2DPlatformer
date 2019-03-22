using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float acceleration = 10.0f;
    [SerializeField] float jumpSpeed = 10.0f;
    [SerializeField] float drag = 2.5f;
    [SerializeField] float epsilon = 0.2f;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float projectileSpeed = 10.0f;
    [SerializeField] Vector2 deathKick = new Vector2(0f, 10f);
    [SerializeField] GameObject mainCamera; // todo - this is temporary, will be moved to the game session manager
    [SerializeField] GameObject projectile;

    private float nextFireTime = 0.0f;
    
    // State
    bool isAlive = true;
    float transformScale = 1.0f; // 1.0 for child, 2.0 for adult and fire

    // References
    Rigidbody2D myRigidBody2D;
    Animator animator;
    Vector2 scaleVector;
    CapsuleCollider2D capsuleColliderCache;
    BoxCollider2D boxColliderCache;
    SpriteRenderer spriteRenderer;

    public Type marioType;
    public enum Type
    {
        CHILD,
        ADULT,
        FIRE
    }

    public State state;
    public enum State
    {
        IDLE,
        WALK,
        TURN,
        JUMP,
        DEATH
    }

    IEnumerator IdleState()
    {
        if (state == State.IDLE)
        {
            CheckForJump();
            CheckForMovement();
            CheckForDeath();
            LaunchProjectile();
        }
        yield return 0;
    }

    IEnumerator WalkState()
    {
        if (state == State.WALK)
        {
            CheckForJump();
            CheckForMovement();
            CheckForDeath();
            LaunchProjectile();
        }
        yield return 0;
    }

    IEnumerator TurnState()
    {
        if (state == State.TURN)
        {
            CheckForJump();
            CheckForMovement();
            CheckForDeath();
            LaunchProjectile();
        }
        yield return 0;
    }

    IEnumerator JumpState()
    {
        if (state == State.JUMP)
        {
            CheckForMovement();
            CheckForDeath();
            LaunchProjectile();
        }
        yield return 0;
    }

    IEnumerator DeathState()
    {
        if (state == State.DEATH)
        {
            isAlive = false;
            myRigidBody2D.velocity = deathKick;
            animator.SetBool("Dying", true);
            mainCamera.GetComponent<CinemachineBrain>().enabled = false;

            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
                if (rigidBody)
                {
                    rigidBody.velocity = Vector2.zero;
                    rigidBody.angularVelocity = 0.0f;

                    MonoBehaviour script = gameObject.GetComponent<MonoBehaviour>();
                    script.enabled = false;

                    Animator animator = gameObject.GetComponent<Animator>();
                    foreach (AnimatorControllerParameter parameter in animator.parameters)
                    {
                        animator.SetBool(parameter.name, false);
                    }
                    animator.StopPlayback();
                }
            }
        }
        yield return 0;
    }

    void NextState()
    {
        switch (state)
        {
            case State.IDLE:
                StartCoroutine("IdleState");
                break;
            case State.WALK:
                StartCoroutine("WalkState");
                break;
            case State.TURN:
                StartCoroutine("TurnState");
                break;
            case State.JUMP:
                StartCoroutine("JumpState");
                break;
            case State.DEATH:
                StartCoroutine("DeathState");
                break;
            default:
                break;
        }
    }
    
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        scaleVector = new Vector2(0f, 0f);
        boxColliderCache = GetComponent<BoxCollider2D>();
        capsuleColliderCache = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        state = State.IDLE;
    }
    
    void Update()
    {
        if (!isAlive) { return; }

        NextState();
    }

    void LaunchProjectile()
    {
        if(marioType == Type.FIRE && CrossPlatformInputManager.GetButtonDown("Fire") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            GameObject fireball = Instantiate(projectile, transform.position, transform.rotation);
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(transform.localScale.x * projectileSpeed, 0.0f);
        }
    }

    void CheckForJump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            state = State.JUMP;
            animator.SetBool("Running", false);
            animator.SetBool("Turning", false);
            animator.SetBool("Jumping", true);

            if (TouchingGround())
            {
                Vector2 jump_velocity = new Vector2(myRigidBody2D.velocity.x, jumpSpeed);
                myRigidBody2D.velocity = jump_velocity;
            }
        }
    }

    void CheckForMovement()
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

            if (direction != Mathf.Sign(myRigidBody2D.velocity.x) && myRigidBody2D.velocity.x != 0.0f)
            {
                change_direction = true;
                myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x / 2, myRigidBody2D.velocity.y);
            }

            if (TouchingGround())
            {
                FlipSprite(direction);
            }
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
            if (state != State.JUMP)
            {
                state = State.IDLE;
            }
            myRigidBody2D.drag = drag;
        }

        // animation
        if (TouchingGround() && state != State.JUMP)
        {
            if (change_direction)
            {
                state = State.TURN;
                animator.SetBool("Turning", apply_movement);
            }
            else
            {
                state = State.WALK;
                bool player_has_hspeed = Mathf.Abs(myRigidBody2D.velocity.x) > epsilon || control_throw != 0.0f;
                animator.SetBool("Turning", false);
                animator.SetBool("Running", player_has_hspeed);
                animator.speed = Mathf.Max(0.5f, 2 * Mathf.Abs(myRigidBody2D.velocity.x) / runSpeed);
            }
        }
    }

    void CheckForDeath()
    {
        if (capsuleColliderCache.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")))
        {
            if (marioType == Type.ADULT || marioType == Type.FIRE)
            {
                ChangeToChild();
            }
            else
            {
                state = State.DEATH;
            }
        }
        else if (capsuleColliderCache.IsTouchingLayers(LayerMask.GetMask("Fall")))
        {
            isAlive = false;
            mainCamera.GetComponent<CinemachineBrain>().enabled = false;
        }
    }

    void FlipSprite( float direction )
    {
        scaleVector.x = direction * transformScale;
        scaleVector.y = transformScale;

        transform.localScale = scaleVector;
    }

    bool TouchingGround()
    {
        return boxColliderCache.IsTouchingLayers(LayerMask.GetMask("Foreground")) || boxColliderCache.IsTouchingLayers(LayerMask.GetMask("Brick"));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            switch (collision.gameObject.tag)
            {
                case "Mushroom":
                    ChangeToFire();
                    break;
                default:
                    break;
            }

            Destroy(collision.gameObject);
        }

        if (collision.otherCollider.GetType() == typeof(BoxCollider2D))
        {
            if (state == State.JUMP)
            {
                state = State.IDLE;
                animator.SetBool("Jumping", false);
            }
        }
    }

    void ChangeToFire()
    {
        if (marioType != Type.FIRE)
        {
            marioType = Type.FIRE;
            transformScale = (marioType == Type.ADULT) ? 1f : 2f;
            transform.localScale = new Vector2(transformScale, transformScale);
            animator.SetLayerWeight(animator.GetLayerIndex("Fire Layer"), 1.0f);
        }
    }

    void ChangeToAdult()
    {
        if (marioType != Type.ADULT)
        {
            marioType = Type.ADULT;
            transformScale = 2f;
            transform.localScale = new Vector2(transformScale, transformScale);
            animator.SetLayerWeight(animator.GetLayerIndex("Adult Layer"), 1.0f);
        }
    }

    void ChangeToChild()
    {
        if (marioType != Type.CHILD)
        {
            marioType = Type.CHILD;
            transformScale = 1f;
            transform.localScale = new Vector2(transformScale, transformScale);
            animator.SetLayerWeight(animator.GetLayerIndex("Adult Layer"), 0.0f);
        }
    }
}
