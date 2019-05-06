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

    public float holdDownTime = 0.35f;
    private float holdDownTimer = 0.0f;

    private CameraController cameraController;

    void Start()
    {
        cameraController = GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>();
        cameraController.SetHorizontalTarget(rb2d.position, facingDirection, false);
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

        // todo: do the friction properly
        //move.x = Mathf.Lerp(0, move.x, holdDownTimer / holdDownTime);
        targetVelocity = move * maxSpeed;

        //if (targetVelocity.x > maxSpeed) targetVelocity.x = maxSpeed;
        //if (targetVelocity.x < -maxSpeed) targetVelocity.x = -maxSpeed;

        UpdateCamera(move.x);
    }

    void UpdateCamera(float xVelocity)
    {
        if (xVelocity == 0f)
        {
            holdDownTimer = 0f;
        }

        if (cameraController)
        {
            if (xVelocity != 0.0f)
            {
                if (facingDirection != Mathf.Sign(xVelocity))
                {
                    holdDownTimer = 0.0f;
                    facingDirection = Mathf.Sign(xVelocity);
                }

                bool isMaxSpeed = holdDownTimer > holdDownTime;
                cameraController.SetHorizontalTarget(rb2d.position, facingDirection, isMaxSpeed);
            }

            cameraController.SetPlayerIsMoving(xVelocity != 0.0f);
        }

        holdDownTimer += Time.deltaTime;
    }

    private void Jump()
    {
        jumpGraceTimer = 0;
        jumpInputBufferTimer = 0;
        velocity.y = jumpSpeedY;
    }
}
