using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : PhysicsObject
{
    public float jumpSpeedY = 7f;
    public float maxSpeed = 7f;
    public float jumpCancelFactor = 0.6f;

    public float jumpGraceTime = 0.2f;

    private float jumpGraceTimer;
    
    void Start()
    {
        
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        
        move.x = CrossPlatformInputManager.GetAxis("Horizontal");

        if (grounded)
        {
            jumpGraceTimer = jumpGraceTime;
        }
        else if (jumpGraceTimer > 0)
        {
            jumpGraceTimer -= Time.deltaTime;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (jumpGraceTimer > 0)
            {
                Jump();
            }
        }
        else if (CrossPlatformInputManager.GetButtonUp("Jump")) // jump cancelling
        {
            if (velocity.y >= 0)
            {
                velocity.y = velocity.y * jumpCancelFactor;
            }
        }

        targetVelocity = move * maxSpeed;
    }

    private void Jump()
    {
        jumpGraceTimer = 0;
        velocity.y = jumpSpeedY;
    }
}
