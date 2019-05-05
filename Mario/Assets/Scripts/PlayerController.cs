using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : PhysicsObject
{
    public float facingDirection = 1.0f;

    public float jumpSpeedY = 7f;
    public float maxSpeed = 7f;
    public float jumpCancelFactor = 0.6f;
    
    public float jumpGraceTime = 0.2f;
    public float jumpInputBufferTime = 0.2f;

    private float jumpGraceTimer;
    private float jumpInputBufferTimer;

    private CameraController cameraController;

    void Start()
    {
        cameraController = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>();
        cameraController.SetHorizontalTarget(rb2d.position.x, rb2d.position, facingDirection, false);
        cameraController.SetPlayerHorizontalSpeed(maxSpeed);
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

        if (cameraController)
        {
            if (move.x != 0.0f)
            {
                bool isMaxSpeed = Mathf.Abs(targetVelocity.x) == maxSpeed;
                facingDirection = Mathf.Sign(move.x);
                cameraController.SetHorizontalTarget(targetVelocity.x + rb2d.position.x, rb2d.position, facingDirection, isMaxSpeed);
            }
        }
        
    }

    private void Jump()
    {
        jumpGraceTimer = 0;
        jumpInputBufferTimer = 0;
        velocity.y = jumpSpeedY;
    }
}
