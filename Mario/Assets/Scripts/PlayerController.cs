using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : PhysicsObject
{
    public float facingDirection = 1.0f;

    public float drag = 0.12f;

    public float jumpSpeedY = 7f;
    public float maxWalkSpeed = 2.5f;

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
        cameraController.SetPlayerHorizontalSpeed(maxWalkSpeed);
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

        // apply horizontal input to speed
        targetVelocity.x = move.x * maxWalkSpeed;

        // apply drag
        targetVelocity.x = Mathf.Lerp(targetVelocity.x, 0, drag);

        // stop
        if (Mathf.Abs(targetVelocity.x) <= 0.1f) targetVelocity.x = 0.0f;

        // face the correct way
        if (facingDirection != Mathf.Sign(move.x))
        {
            holdDownTimer = 0.0f;
            facingDirection = Mathf.Sign(move.x);
        }

        // limit the speed
        //targetVelocity.x = Mathf.Min(Mathf.Abs(targetVelocity.x), maxHorizontalSpeed) * facingDirection;
        
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (cameraController)
        {
            if (targetVelocity.x != 0.0f)
            {
                bool isMaxSpeed = Mathf.Abs(targetVelocity.x) == Mathf.Lerp(maxWalkSpeed, 0, drag);
                cameraController.SetHorizontalTarget(rb2d.position, facingDirection, isMaxSpeed);
            }

            cameraController.SetPlayerIsMoving(targetVelocity.x != 0.0f);
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
