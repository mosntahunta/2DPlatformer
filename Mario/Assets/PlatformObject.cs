using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : MonoBehaviour
{
    public bool movingPlatform = false;
    public bool disappearingPlatform = false;

    public float patrolTime = 3f;
    private float patrolTimer = 0f;

    public float horizontalVelocity = 3f;
    public float verticalVelocity = 3f;

    public float disappearDelayTime = 0.5f;
    private float disappearDelayTimer = 0f;

    public float disappearTime = 5f;

    private Vector2 targetVelocity;

    private Rigidbody2D rb2d;
    private Collider2D collider2d;

    void OnEnable()
    {
        patrolTimer = patrolTime;
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingPlatform)
        {
            if (patrolTimer > 0)
            {
                patrolTimer -= Time.deltaTime;
                targetVelocity.x = horizontalVelocity;
                targetVelocity.y = verticalVelocity;
            }
            else
            {
                horizontalVelocity = -horizontalVelocity;
                verticalVelocity = -verticalVelocity;
                patrolTimer = patrolTime;
            }
        }

        if (disappearingPlatform)
        {
            if (disappearDelayTimer > 0)
            {
                disappearDelayTimer -= Time.deltaTime;

                if (disappearDelayTimer <= 0)
                {
                    StartCoroutine("DisappearCoroutine");
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (movingPlatform)
        {
            Vector2 deltaPosition = targetVelocity * Time.deltaTime;
            
            TryPushObjects(deltaPosition);
            TryPullObjects(deltaPosition);

            rb2d.position = rb2d.position + deltaPosition;
        }
    }

    public void BeginDisappear()
    {
        if (disappearDelayTimer <= 0)
        {
            disappearDelayTimer = disappearDelayTime;
        }
    }

    // 
    // Hide and disable the collider of the platform
    //
    private IEnumerator DisappearCoroutine()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        collider2d.enabled = false;

        yield return new WaitForSeconds(disappearTime);
        
        renderer.enabled = true;
        collider2d.enabled = true;
    }

    private void TryPushObjects(Vector2 deltaPosition)
    {
        float colliderOffsetX = targetVelocity.normalized.x * collider2d.bounds.size.x / 2;
        float colliderOffsetY = targetVelocity.normalized.y * collider2d.bounds.size.y / 2;

        Vector2 rayStartX = new Vector2(rb2d.position.x + colliderOffsetX, rb2d.position.y);
        Vector2 rayStartY = new Vector2(rb2d.position.x, rb2d.position.y + colliderOffsetY);

        Vector2 sizeX = new Vector2(0.05f, collider2d.bounds.size.y);
        Vector2 sizeY = new Vector2(collider2d.bounds.size.x, 0.05f);

        Vector2 directionX = new Vector2(targetVelocity.normalized.x, 0);
        Vector2 directionY = new Vector2(0, targetVelocity.normalized.y);

        RaycastHit2D[] hitsX = Physics2D.BoxCastAll(rayStartX, sizeX, 0, directionX, 0.05f, LayerMask.GetMask("Player", "Enemy"));
        RaycastHit2D[] hitsY = Physics2D.BoxCastAll(rayStartY, sizeY, 0, directionY, 0.05f, LayerMask.GetMask("Player", "Enemy"));
        
        for (int i = 0; i < hitsX.Length; i++)
        {
            hitsX[i].collider.GetComponent<Rigidbody2D>().position += new Vector2(deltaPosition.x, 0);
        }

        for (int i = 0; i < hitsY.Length; i++)
        {
            hitsY[i].collider.GetComponent<Rigidbody2D>().position += new Vector2(0, deltaPosition.y);
        }
    }

    private void TryPullObjects(Vector2 deltaPosition)
    {
        // Currently other objects can only mount a platform from above but we may allow mounting
        // on any side of the platform later on.
        float colliderOffsetY = collider2d.bounds.size.y / 2;
        Vector2 rayStartY = new Vector2(rb2d.position.x, rb2d.position.y + colliderOffsetY);
        Vector2 sizeY = new Vector2(collider2d.bounds.size.x, 0.05f);
        Vector2 directionY = new Vector2(0, 1f);

        RaycastHit2D[] hitsX = Physics2D.BoxCastAll(rayStartY, sizeY, 0, directionY, 0.1f, LayerMask.GetMask("Player", "Enemy"));
        for (int i = 0; i < hitsX.Length; i++)
        {
            hitsX[i].collider.GetComponent<Rigidbody2D>().position += new Vector2(deltaPosition.x, 0);
        }

        if (targetVelocity.y < 0)
        {
            RaycastHit2D[] hitsY = Physics2D.BoxCastAll(rayStartY, sizeY, 0, directionY, 0.1f, LayerMask.GetMask("Player", "Enemy"));
            for (int i = 0; i < hitsY.Length; i++)
            {
                hitsY[i].collider.GetComponent<Rigidbody2D>().position += new Vector2(0, deltaPosition.y);
            }
        }
    }
}
