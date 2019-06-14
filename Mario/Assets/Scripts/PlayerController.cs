using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : PhysicsObject
{
    public int facingDirection = 1;

    public float drag = 0.12f;
    
    public float horizontalAcceleration = 1000f;
    public float horizontalDeceleration = 400f;
    public float maxHorizontalSpeed = 3f;
    public float minimumHorizontalSpeed = 0.1f;
    public float horizontalSpeed = 0.0f;
    
    public float jumpSpeedY = 7f;
    public float superJumpSpeedY = 10f;

    public float jumpGraceTime = 0.2f;
    public float jumpInputBufferTime = 0.2f;
    public float jumpVarTime = 0f;
    public float jumpVarCeilingGrace = 0.05f;

    public float jumpHBoost = 10f;
    public float superJumpH = 50f;

    private float jumpGraceTimer;
    private float jumpInputBufferTimer;
    private float jumpVarTimer = 0f;

    private float jumpYPos = 0f;

    private float moveX = 0f;
    private float forceMoveX = 0f;
    private float forceMoveXTimer = 0f;

    public float wallJumpHSpeed = 5f;
    public float wallJumpForceTime = 0.16f;

    public float wallSlideStartMax = 1.25f;
    public float wallSlideTime = 1.2f;
    private float wallSlideTimer = 0f;
    private float wallSlideDirection = 0f;

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

    public float slashCooldownTime = 0.25f;
    private float slashCooldownTimer = 0f;

    public float slashAttackTime = 0.10f;
    private float slashAttackTimer = 0f;

    public float platformCameraMovementRange = 1f;

    private float lastLandedPlatformHeight = float.NegativeInfinity;

    private float modifiedMaxFall = 0f;

    // state references
    private int normalState = 0;
    private int dashState = 1;

    private CameraController cameraController;
    private StateMachine stateMachine;

    private BoxCollider2D hitBox;
    private Animator animator;

    private Vector2 scaleVector;

    MeleeAttack meleeAttack;
    
    protected override void Start()
    {
        base.Start();

        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        cameraController.SetHorizontalTarget(rb2d.position, facingDirection, false);

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.Initialise(2);
        stateMachine.SetCallbacks(normalState, null, null, NormalUpdate, null);
        stateMachine.SetCallbacks(dashState, DashBegin, DashExit, DashUpdate, DashCoroutine);
        stateMachine.SetState(normalState);

        hitBox = GetComponent<BoxCollider2D>();
        hitBox.edgeRadius = 0.03f;

        animator = GetComponent<Animator>();
        scaleVector = new Vector2(1f, 1f);

        meleeAttack = GetComponentInChildren<MeleeAttack>();
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

        if (slashCooldownTimer > 0)
        {
            slashCooldownTimer -= Time.deltaTime;
        }

        if (slashAttackTimer > 0)
        {
            slashAttackTimer -= Time.deltaTime;
        }
        else
        {
            meleeAttack.Sheathe();
            animator.SetBool("MeleeAttacking", false);
        }

        //Wall Slide
        if (wallSlideDirection != 0)
        {
            wallSlideTimer = Mathf.Max(wallSlideTimer - Time.deltaTime, 0);
            wallSlideDirection = 0;
        }

        if (playerHurtTimer > 0)
        {
            playerHurtTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        // Player dies when crushed between ground and platform
        if (Physics2D.OverlapCircle(transform.position, hitBox.bounds.size.x / 2, LayerMask.GetMask("Ground", "Platform")))
        {
            // this has unintended consequences where if the player gets stuck in a ground/platform collider, he will still die.
            Debug.Log("overlap detected");
            OnDeath();
        }
    }

    // Normal state
    private int NormalUpdate()
    {
        targetVelocity = Vector2.zero;

        //
        // todo - create weapon classes to define the attack speed, type of attack, etc
        //
        // Melee Attack
        if (CrossPlatformInputManager.GetButton("MeleeAttack"))
        {
            if (slashCooldownTimer <= 0)
            {
                MeleeAttack();
                slashCooldownTimer = slashCooldownTime;
            }
        }

        // Range Attack
        if (CrossPlatformInputManager.GetButton("RangeAttack"))
        {
            if (shootTimer <= 0)
            {
                RangeAttack();
                shootTimer = shootTime;
            }
        }

        
        // Force Move X
        if (forceMoveXTimer > 0)
        {
            forceMoveXTimer -= Time.deltaTime;
            moveX = forceMoveX;
        }
        else
        {
            // Horizontal Input
            if (CrossPlatformInputManager.GetButton("Horizontal"))
            {
                moveX = Mathf.Sign(CrossPlatformInputManager.GetAxis("Horizontal"));

                if (grounded)
                {
                    animator.SetBool("Running", true);
                }
            }
            else
            {
                moveX = 0;
                animator.SetBool("Running", false);
            }
        }

        if (grounded)
        {
            // Vertical Input
            if (CrossPlatformInputManager.GetButton("Vertical"))
            {
                // todo: manual camera control doesn't work when the player is near the boundaries in the opposite direction
                float verticalCameraOffset = 0f;
                float yInputDir = Mathf.Sign(CrossPlatformInputManager.GetAxis("Vertical"));
                if (yInputDir < 0)
                {
                    verticalCameraOffset -= 3f;

                    if (!ducking)
                    {
                        Duck();
                    }
                }
                else
                {
                    verticalCameraOffset += 3f;
                }
                
                cameraController.SetVerticalOffset(verticalCameraOffset);
            }
            else 
            {
                cameraController.SetVerticalOffset(0f);

                if (ducking)
                {
                    Unduck();
                }
            }

            animator.SetBool("Jumping", false);
        }
        else
        {
            // falling off the ground
            animator.SetBool("Running", false);
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
            facingDirection = (int)Mathf.Sign(horizontalSpeed);
        }
        
        // Dashing
        if (CanDash)
        {
            return dashState;
        }

        modifiedMaxFall = maxFall;

        // Wall Slide
        if (!grounded && moveX == facingDirection)
        {
            WallSlide();
        }

        // Gravity
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
            else if (WallJumpCheck(1))
            {
                WallJump(-1);
            }
            else if (WallJumpCheck(-1))
            {
                WallJump(1);
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
            animator.SetBool("Ducking", ducking);
        }
    }

    private void Unduck()
    {
        if (hitBox)
        {
            hitBox.size = new Vector2(hitBox.size.x, hitBox.size.y * 2);
            hitBox.offset = new Vector2(0, 0);

            ducking = false;
            animator.SetBool("Ducking", ducking);
        }
    }

    private void MeleeAttack()
    {
        meleeAttack.Unsheathe();
        slashAttackTimer = slashAttackTime;
        animator.SetBool("MeleeAttacking", true);
    }

    private void RangeAttack()
    {
        GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("PlayerBullet");
        if (bullet != null)
        {
            // todo fix this to get proper direction
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
        animator.SetBool("Dashing", true);
        dashCooldownTimer = dashCooldownTime;

        if (ducking)
        {
            Unduck();
        }
    }

    private void DashExit()
    {
        animator.SetBool("Dashing", false);
    }

    private int DashUpdate()
    {
        ApplyGravity();

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

        wallSlideTimer = wallSlideTime;

        if (ducking)
        {
            Unduck();
        }

        jumpYPos = rb2d.position.y;

        // Update and fixed update are not always 1-1, thus you can still sometimes
        // duck while jumping due to grounded not being set to false in fixedupdate.
        grounded = false;
        velocity.y = jumpSpeedY;
        horizontalSpeed += moveX * jumpHBoost;

        animator.SetBool("Jumping", true);
    }

    private void SuperJump()
    {
        jumpGraceTimer = 0;
        jumpInputBufferTimer = 0;
        jumpVarTimer = jumpVarTime;

        wallSlideTimer = wallSlideTime;

        if (ducking)
        {
            Unduck();
        }

        jumpYPos = rb2d.position.y;

        grounded = false;
        velocity.y = superJumpSpeedY;
        horizontalSpeed = facingDirection * superJumpH;

        animator.SetBool("Jumping", true);
    }

    private void WallJump(int direction)
    {
        jumpGraceTimer = 0;
        jumpVarTimer = jumpVarTime;

        wallSlideTimer = wallSlideTime;

        if (moveX != 0)
        {
            forceMoveX = direction;
            forceMoveXTimer = wallJumpForceTime;
        }

        horizontalSpeed = wallJumpHSpeed * direction;
        velocity.y = jumpSpeedY;
    }

    private bool WallJumpCheck(int direction)
    {
        float wallJumpCheckDist = 0.1f;
        float colliderOffsetX = facingDirection * hitBox.bounds.size.x / 2;

        Vector2 rayStart = new Vector2(transform.position.x + colliderOffsetX, transform.position.y);
        Vector2 size = new Vector2(wallJumpCheckDist, hitBox.bounds.size.y);
        Vector2 directionVector = new Vector2(direction, 0f);
        
        RaycastHit2D hit = Physics2D.BoxCast(rayStart, size, 0,  directionVector, wallJumpCheckDist, LayerMask.GetMask("Ground", "Platform"));
        
        return hit.collider != null;
    }

    private void WallSlide()
    {
        if (velocity.y <= 0f && wallSlideTimer > 0 && WallJumpCheck(facingDirection))
        {
            ducking = false;
            wallSlideDirection = facingDirection;
        }

        if (wallSlideDirection != 0)
        {
            if (wallSlideTimer > wallSlideTime * .5f)
            {
                wallSlideTimer = wallSlideTime * .5f;
            }

            modifiedMaxFall = Mathf.Lerp(maxFall, wallSlideStartMax, wallSlideTimer / wallSlideTime);
        }
    }

    protected override void OnCollision(Collider2D collider, Vector2 contactPosition)
    {
        GameObject collidedObject = collider.gameObject;
        string layerName = LayerMask.LayerToName(collidedObject.layer);
        string tagName = collidedObject.tag;

        if (ceilingCollision)
        {
            if (jumpVarTimer < jumpVarTime - jumpVarCeilingGrace)
            {
                jumpVarTimer = 0;
            }
        }
        
        if (grounded)
        {
            if (tagName == "Platform")
            {
                PlatformObject platformScript = collidedObject.GetComponent<PlatformObject>();

                if (!platformScript.movingPlatform)
                {
                    OnLanded();
                }
                
                if (platformScript.disappearingPlatform)
                {
                    platformScript.BeginDisappear();
                }
            }
            else
            {
                cameraController.SetVerticalState(CameraController.VerticalState.PositionLock);
                lastLandedPlatformHeight = float.NegativeInfinity;
            }

            animator.SetBool("Jumping", false);
        }
        else if (layerName == "Enemy")
        {
            OnHit(collider, contactPosition);
        }
        else if (layerName == "Trap")
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
        facingDirection = (int)Mathf.Sign(horizontalSpeed);

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

    protected override void ApplyGravity()
    {
        velocity.y = Mathf.MoveTowards(velocity.y, -modifiedMaxFall, gravityModifier * Mathf.Abs(Physics2D.gravity.y) * Time.deltaTime);
    }

    protected override void PositionIsSet()
    {
        // flip the player sprite if needed
        if (Mathf.Sign(scaleVector.x) != facingDirection)
        {
            scaleVector.x = facingDirection;
            transform.localScale = scaleVector;
        }

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
                    cameraController.SetVerticalState(CameraController.VerticalState.PositionLock);
                    lastLandedPlatformHeight = float.NegativeInfinity;
                }
            }
            
            if (cameraController.GetVerticalState() == CameraController.VerticalState.PositionLock)
            {
                cameraController.SetVerticalTarget(rb2d.position, 0.0f);
            }
        }
    }

    // Item pickup
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Weapon")
        {
            canShoot = true;
        }
    }
}
