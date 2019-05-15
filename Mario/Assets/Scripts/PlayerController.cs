using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : PhysicsObject
{
    public float facingDirection = 1.0f;

    public float drag = 0.12f;
    
    public float horizontalAcceleration = 1000f;
    public float maxHorizontalSpeed = 3f;
    public float minimumHorizontalSpeed = 0.1f;
    private float horizontalSpeed = 0.0f;

    public float platformCameraMovementRange = 1f;

    public float jumpSpeedY = 7f;

    public float jumpGraceTime = 0.2f;
    public float jumpInputBufferTime = 0.2f;
    public float jumpVarTime = 0f;

    private float jumpGraceTimer;
    private float jumpInputBufferTimer;
    private float jumpVarTimer = 0f;

    private CameraController cameraController;
    private float lastLandedPlatformHeight = float.NegativeInfinity;

    void Start()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        cameraController.SetHorizontalTarget(rb2d.position, facingDirection, false);
        cameraController.SetPlayerHorizontalSpeed(maxHorizontalSpeed);
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

        ApplyGravity();

        // variable jumping
        if (jumpVarTimer > 0)
        {
            jumpVarTimer -= Time.deltaTime;

            if (CrossPlatformInputManager.GetButton("Jump"))
            {
                velocity.y = Mathf.Max(velocity.y, jumpSpeedY);
            }
            else
            {
                jumpVarTimer = 0;
            }
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            jumpInputBufferTimer = jumpInputBufferTime;
            if (jumpGraceTimer > 0)
            {
                Jump();
            }
        }
        
        // apply horizontal acceleration over time
        horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, move.x * maxHorizontalSpeed, horizontalAcceleration * Time.deltaTime);

        // apply drag
        horizontalSpeed = Mathf.Lerp(horizontalSpeed, 0, drag);

        // stop if speed is below the minimum horizontal speed
        if (Mathf.Abs(horizontalSpeed) <= minimumHorizontalSpeed) horizontalSpeed = 0.0f;

        // face the correct way
        if (facingDirection != Mathf.Sign(horizontalSpeed))
        {
            facingDirection = Mathf.Sign(horizontalSpeed);
        }

        // limit the speed
        horizontalSpeed = Mathf.Min(Mathf.Abs(horizontalSpeed), maxHorizontalSpeed) * facingDirection;

        targetVelocity.x = horizontalSpeed;
    }

    protected override void OnLanded(int layer)
    {
        string layerName = LayerMask.LayerToName(layer);
        bool positionWithinRange = rb2d.position.y >= lastLandedPlatformHeight - platformCameraMovementRange && 
                                   rb2d.position.y <= lastLandedPlatformHeight + platformCameraMovementRange;

        if (layerName == "Platform" && !positionWithinRange)
        {
            lastLandedPlatformHeight = rb2d.position.y;
            cameraController.SetVerticalState(CameraController.VerticalState.PlatformLock);
            cameraController.SetVerticalTarget(rb2d.position, 2f); // todo: create public property for delay
        }
        else if (layerName == "Ground")
        {
            lastLandedPlatformHeight = rb2d.position.y;
        }
    }

    protected override void PositionIsSet()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (cameraController)
        {
            if (targetVelocity.x != 0.0f)
            {
                bool isMaxSpeed = Mathf.Abs(targetVelocity.x) == Mathf.Lerp(maxHorizontalSpeed, 0, drag);
                cameraController.SetHorizontalTarget(rb2d.position, facingDirection, isMaxSpeed);
            }

            if (cameraController.GetVerticalState() == CameraController.VerticalState.PlatformLock)
            {
                if (rb2d.position.y < lastLandedPlatformHeight - platformCameraMovementRange)
                {
                    cameraController.SetVerticalState(CameraController.VerticalState.FreeMoving);
                }
            }
            
            if (cameraController.GetVerticalState() == CameraController.VerticalState.FreeMoving)
            {
                cameraController.SetVerticalTarget(rb2d.position, 0.0f);
            }
            cameraController.SetPlayerIsMoving(targetVelocity.x != 0.0f);
        }
    }

    private void Jump()
    {
        jumpGraceTimer = 0;
        jumpInputBufferTimer = 0;
        jumpVarTimer = jumpVarTime;

        velocity.y = jumpSpeedY;
    }
}
