using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1.0f;
    public float maxFall = 20f;
    
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected Vector2 targetVelocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected Vector2 positionOffset;

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    protected Collider2D mountParent; // e.g. moving platforms should also move the player
    protected Vector2 previousMountPosition;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        positionOffset = new Vector2(0, 0);
        previousMountPosition = Vector2.negativeInfinity;
    }
    
    protected virtual void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.useLayerMask = true;
    }
    
    void FixedUpdate()
    {
        // Calculate the offset position caused by a mounted parent object
        if (mountParent != null)
        {
            if (HasCollisionDataFor(mountParent))
            {
                Rigidbody2D mountRb2d = mountParent.GetComponent<Rigidbody2D>();
                positionOffset.x = mountRb2d.position.x - previousMountPosition.x;
                positionOffset.y = mountRb2d.position.y - previousMountPosition.y;

                previousMountPosition = mountRb2d.position;
            }
            else
            {
                mountParent = null;
                previousMountPosition = Vector2.negativeInfinity;
            }
        }

        // Apply offset position
        rb2d.position = rb2d.position + positionOffset;
        positionOffset.Set(0, 0);
        
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

        velocity.x = targetVelocity.x;
        
        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);
        
        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
        
        PositionIsSet();
    }

    protected virtual void PositionIsSet()
    {

    }

    protected virtual void OnCollision(Collider2D collider, Vector2 contactPosition)
    {

    }

    protected virtual void ApplyGravity()
    {

    }

    protected bool HasCollisionDataFor(Collider2D other)
    {
        for (int i = 0; i < hitBufferList.Count; i++)
        {
            if (hitBufferList[i].collider == other)
            {
                return true;
            }
                
        }

        return false;
    }

    //
    // Move in the specified velocity.
    //
    // Takes account for the collision with the ground or slope.
    // To be called by FixedUpdate().
    //
    // @param move
    //  Vector to move to.
    //
    // @param yMovement
    //  Whether or not there is a y-axis movement.
    //
    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        Collider2D finalHitCollider = null; 

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                // Get the closest collision distance
                float modifiedDistance = hitBufferList[i].distance - shellRadius;

                if (modifiedDistance < distance)
                {
                    finalHitCollider = hitBufferList[i].collider;
                    distance = modifiedDistance;
                }
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;

        if (finalHitCollider != null)
        {
            OnCollision(finalHitCollider, rb2d.position);
        }
    }
}
