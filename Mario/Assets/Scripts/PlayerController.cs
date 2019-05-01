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
    public float jumpInputBufferTime = 0.2f;

    private float jumpGraceTimer;
    private float jumpInputBufferTimer;

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

            if (jumpInputBufferTimer > 0)
            {
                Jump();
            }
        }
        else if (jumpGraceTimer > 0)
        {
            jumpGraceTimer -= Time.deltaTime;
        }

        if (jumpInputBufferTimer > 0)
        {
            jumpInputBufferTimer -= Time.deltaTime;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            jumpInputBufferTimer = jumpInputBufferTime;
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
        jumpInputBufferTimer = 0;
        velocity.y = jumpSpeedY;
    }
}
