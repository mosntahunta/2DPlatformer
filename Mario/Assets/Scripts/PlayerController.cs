using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : PhysicsObject
{
    public float facingDirection = 1.0f;

    public float drag = 0.12f;
    
    public float horizontalAcceleration = 1000f;
    public float horizontalDeceleration = 400f;
    public float maxHorizontalSpeed = 3f;
    public float minimumHorizontalSpeed = 0.1f;
    public float horizontalSpeed = 0.0f;

    private float moveX = 0f;

    public float jumpSpeedY = 7f;
    public float superJumpSpeedY = 10f;

    public float jumpGraceTime = 0.2f;
    public float jumpInputBufferTime = 0.2f;
    public float jumpVarTime = 0f;

    public float jumpHBoost = 10f;
    public float superJumpH = 50f;

    private float jumpGraceTimer;
    private float jumpInputBufferTimer;
    private float jumpVarTimer = 0f;

    private bool ducking = false;

    public float dashSpeedX = 20f;
    public float dashTime = 0.15f;

    public float dashCooldownTime = 0.2f;
    private float dashCooldownTimer = 0f;

    public float playerHurtTime = 1.5f;
    private float playerHurtTimer = 0f;

    public float enemyContactKnockbackSpeed = 10f;

    // temporary for now, later on, there will be inventory system for guns and items, etc
    public bool canShoot = false;

    public float shootTime = 0.1f;
    private float shootTimer = 0f;

    public float platformCameraMovementRange = 1f;

    private float lastLandedPlatformHeight = float.NegativeInfinity;

    // state references
    private int normalState = 0;
    private int dashState = 1;

    private CameraController cameraController;
    private StateMachine stateMachine;

    private BoxCollider2D hitBox;
    
    protected override void Start()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        cameraController.SetHorizontalTarget(rb2d.position, facingDirection, false);

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.Initialise(2);
        stateMachine.SetCallbacks(normalState, null, null, NormalUpdate, null);
        stateMachine.SetCallbacks(dashState, DashBegin, DashExit, DashUpdate, DashCoroutine);
        stateMachine.SetState(normalState);

        hitBox = GetComponent<BoxCollider2D>();
        hitBox.edgeRadius = 0.03f;
    }

    void Update()
    {
        // Save and Load for testing purposes. This will be replaced when we implement the UI.
        if (CrossPlatformInputManager.GetButtonUp("Save"))
        {
            Debug.Log("save");
            GameModel.SharedInstance.SaveData();
        }

        if (CrossPlatformInputManager.GetButtonUp("Load"))
        {
            Debug.Log("load");
            GameModel.SharedInstance.LoadData();
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        if (playerHurtTimer > 0)
        {
            playerHurtTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    // Normal state
    private int NormalUpdate()
    {
        targetVelocity = Vector2.zero;
        moveX = 0;

        // Shoot
        if (CrossPlatformInputManager.GetButton("Fire") && canShoot)
        {
            if (shootTimer <= 0)
            {
                Shoot();
                shootTimer = shootTime;
            }
        }

        // Horizontal Input
        if (CrossPlatformInputManager.GetButton("Horizontal"))
        {
            moveX = Mathf.Sign(CrossPlatformInputManager.GetAxis("Horizontal"));
        }

        if (grounded)
        {
            // Vertical Input
            if (CrossPlatformInputManager.GetButton("Vertical"))
            {
                float yInputDir = Mathf.Sign(CrossPlatformInputManager.GetAxis("Vertical"));

                if (yInputDir < 0 && !ducking)
                {
                    Duck();
                }
            }
            else if (ducking)
            {
                Unduck();
            }
        }

        // approach the max speed with acceleration or deceleration
        if (Mathf.Abs(horizontalSpeed) > maxHorizontalSpeed && Mathf.Sign(horizontalSpeed) == moveX)
        {
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, moveX * maxHorizontalSpeed, horizontalDeceleration * Time.deltaTime);
        }
        else
        {
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, moveX * maxHorizontalSpeed, horizontalAcceleration * Time.deltaTime);
        }
        
        // stop if speed is below the minimum horizontal speed
        if (Mathf.Abs(horizontalSpeed) <= minimumHorizontalSpeed) horizontalSpeed = 0.0f;

        // face the correct way
        if (horizontalSpeed != 0 && facingDirection != Mathf.Sign(horizontalSpeed))
        {
            facingDirection = Mathf.Sign(horizontalSpeed);
        }
        
        // Dashing
        if (CanDash)
        {
            return dashState;
        }

        ApplyGravity();

        // Jumping
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

        targetVelocity.x = horizontalSpeed;

        return normalState;
    }

    // todo - use the get{} pattern
    private void Duck()
    {
        if (hitBox)
        {
            hitBox.size = new Vector2(hitBox.size.x, hitBox.size.y / 2);
            hitBox.offset = new Vector2(0, -hitBox.size.y / 2);
            ducking = true;
        }
    }

    private void Unduck()
    {
        if (hitBox)
        {
            hitBox.size = new Vector2(hitBox.size.x, hitBox.size.y * 2);
            hitBox.offset = new Vector2(0, 0);
            ducking = false;
        }
    }
    
    private void Shoot()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("PlayerBullet");
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(facingDirection, 0, 0));
            bullet.SetActive(true);
        }
    }

    // Dash state
    public bool CanDash
    {
        get
        {
            return CrossPlatformInputManager.GetButtonDown("Dash") && dashCooldownTimer <= 0 && jumpGraceTimer > 0;
        }
    }

    private void DashBegin()
    {
        dashCooldownTimer = dashCooldownTime;

        if (ducking)
        {
            Unduck();
        }
    }

    private void DashExit()
    {
    }

    private int DashUpdate()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            SuperJump();
            return normalState;
        }

        return dashState;
    }

    private IEnumerator DashCoroutine()
    {
        yield return null;

        // todo: perhaps use the aim vector to determine dash direction, rather than facing
        targetVelocity.x = dashSpeedX * facingDirection;

        yield return new WaitForSeconds(dashTime);

        stateMachine.SetState(normalState);
    }

    private void Jump()
    {
        jumpGraceTimer = 0;
        jumpInputBufferTimer = 0;
        jumpVarTimer = jumpVarTime;

        if (ducking)
        {
            Unduck();
        }

        // Update and fixed update are not always 1-1, thus you can still sometimes
        // duck while jumping due to grounded not being set to false in fixedupdate.
        grounded = false;
        velocity.y = jumpSpeedY;
        horizontalSpeed += moveX * jumpHBoost;
    }

    private void SuperJump()
    {
        jumpGraceTimer = 0;
        jumpInputBufferTimer = 0;
        jumpVarTimer = jumpVarTime;

        if (ducking)
        {
            Unduck();
        }
        
        grounded = false;
        velocity.y = superJumpSpeedY;
        horizontalSpeed = facingDirection * superJumpH;
    }
    
    protected override void OnCollision(Collider2D collider, Vector2 contactPosition)
    {
        GameObject collidedObject = collider.gameObject;
        string layerName = LayerMask.LayerToName(collidedObject.layer);

        // todo: Every type of ground uses the platform-lock camera for now. Later, we may also require the free moving camera.
        // Study the super mario world camera more closely
        if (grounded && (layerName == "Ground" || layerName == "Platform"))
        {
            OnLanded();
        }

        if (layerName == "Enemy")
        {
            OnHit(collider, contactPosition);
        }

        if (layerName == "Trap")
        {
            OnDeath();
        }
    }

    private void OnLanded()
    {
        bool positionWithinRange = rb2d.position.y >= lastLandedPlatformHeight - platformCameraMovementRange &&
                                   rb2d.position.y <= lastLandedPlatformHeight + platformCameraMovementRange;

        if (!positionWithinRange)
        {
            lastLandedPlatformHeight = rb2d.position.y;
            cameraController.SetVerticalState(CameraController.VerticalState.PlatformLock);
            cameraController.SetVerticalTarget(rb2d.position, 2f); // todo: create public property for delay
        }
    }

    private void OnHit(Collider2D collider, Vector2 contactPosition)
    {
        horizontalSpeed = (contactPosition.x < collider.bounds.center.x) ? -enemyContactKnockbackSpeed : enemyContactKnockbackSpeed;
        facingDirection = Mathf.Sign(horizontalSpeed);

        gameObject.layer = LayerMask.NameToLayer("PlayerHurt");
        playerHurtTimer = playerHurtTime;

        GameModel.SharedInstance.playerData.currentLives--;

        if (GameModel.SharedInstance.playerData.currentLives == 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        SceneController.SharedInstance.ReloadCurrentScene();
    }

    protected override void PositionIsSet()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (cameraController)
        {
            // Horizontal
            if (targetVelocity.x != 0.0f)
            {
                bool playerIsMoving = Mathf.Abs(targetVelocity.x) >= maxHorizontalSpeed;
                cameraController.SetHorizontalTarget(rb2d.position, facingDirection, playerIsMoving);

                if (playerIsMoving)
                {
                    cameraController.SetPlayerHorizontalSpeed(Mathf.Abs(targetVelocity.x));
                }
            }

            // Vertical
            if (cameraController.GetVerticalState() == CameraController.VerticalState.PlatformLock)
            {
                if (rb2d.position.y < lastLandedPlatformHeight - platformCameraMovementRange)
                {
                    cameraController.SetVerticalState(CameraController.VerticalState.FreeMoving);
                }
            }
            
            if (cameraController.GetVerticalState() == CameraController.VerticalState.FreeMoving)
            {
                cameraController.SetVerticalSpeed(Mathf.Abs(velocity.y));
                cameraController.SetVerticalTarget(rb2d.position, 0.0f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.gameObject.tag == "Weapon")
        {
            canShoot = true;
        }
    }
}
