using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minimumHSpeed = 0.5f;
    public float turnAroundHSpeed = 2f;
    public float followFromBehindHBoost = 0.5f;

    public float forwardDistance = 1f;
    public float slowdownFactor = 5f;
    public float cameraDrag = 0.4f;

    public float cameraCollisionLookahead = 1f;

    public float positionCameraVerticalSpeed = 8f;
    public float platformCameraVerticalSpeed = 7f;
    private float cameraVerticalSpeed = 3f;
    
    private float playerHorizontalSpeed = 0f;

    private float playerDirection = 0f;
    private bool playerIsMoving = false;
    
    public float cameraTurnTime = 0.5f;
    private float cameraTurnTimer = 0.0f;

    private float verticalOffset = 0.0f;

    private Camera cameraCache;

    Vector2 playerPosition;
    Vector3 cameraTarget;

    VerticalState verticalState;
    public enum VerticalState
    {
        GroundLock,
        TopLock,
        PlatformLock,
        PositionLock
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;
        verticalState = VerticalState.PositionLock;
        cameraCache = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        RaycastHit2D horizontalHitBox;
        RaycastHit2D verticalHitBox;

        if (cameraTarget.x > transform.position.x)
        {
            horizontalHitBox = CheckForBoundary(new Vector2(1, 0));
        }
        else
        {
            horizontalHitBox = CheckForBoundary(new Vector2(-1, 0));
        }

        if (cameraTarget.y + verticalOffset > transform.position.y)
        {
            verticalHitBox = CheckForBoundary(new Vector2(0, 1));
        }
        else
        {
            verticalHitBox = CheckForBoundary(new Vector2(0, -1));
        }

        if (horizontalHitBox.collider == null)
        {
            HorizontalMovement();
        }

        if (verticalHitBox.collider == null)
        {
            VerticalMovement();
        }
    }

    void HorizontalMovement()
    {
        // The horizontal movement uses the `Dual Forward Focus` camera to ensure that the player sees
        // more in the direction he is facing.
        float cameraHorizontalSpeed = minimumHSpeed;
        float cameraToTarget = Mathf.Abs(cameraTarget.x - transform.position.x);
        float playerToTarget = Mathf.Abs(cameraTarget.x - playerPosition.x);
        
        if (cameraToTarget < playerToTarget)
        {
            cameraHorizontalSpeed = Mathf.Lerp(minimumHSpeed, playerHorizontalSpeed, cameraToTarget / playerToTarget);
        }
        else if (cameraToTarget > playerToTarget)
        {
            cameraHorizontalSpeed = playerHorizontalSpeed;

            if (playerIsMoving)
            {
                cameraHorizontalSpeed += minimumHSpeed;
            }
        }
        else
        {
            cameraHorizontalSpeed = playerHorizontalSpeed;
        }

        // limit the camera speed
        if (cameraHorizontalSpeed < minimumHSpeed) cameraHorizontalSpeed = minimumHSpeed;

        // stop the camera shaking when the player turns around
        if (cameraTurnTimer > 0)
        {
            cameraTurnTimer -= Time.deltaTime;
            cameraHorizontalSpeed = Mathf.Lerp(cameraHorizontalSpeed, 0.0f, cameraTurnTimer / cameraTurnTime);
        }

        Vector3 horizontalTarget = new Vector3(cameraTarget.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, horizontalTarget, Time.deltaTime * cameraHorizontalSpeed);
    }

    void VerticalMovement()
    {
        Vector3 verticalTarget = new Vector3(transform.position.x, cameraTarget.y + verticalOffset, transform.position.z);
        if (verticalState == VerticalState.PositionLock)
        {
            transform.position = Vector3.Lerp(transform.position, verticalTarget, Time.deltaTime * positionCameraVerticalSpeed);
        }
        else if (verticalState == VerticalState.PlatformLock)
        {
            transform.position = Vector3.MoveTowards(transform.position, verticalTarget, Time.deltaTime * platformCameraVerticalSpeed);
        }
    }

    RaycastHit2D CheckForBoundary(Vector2 direction)
    {
        float screenHeightHalf = cameraCache.orthographicSize;
        float screenWidthHalf = cameraCache.aspect * screenHeightHalf;

        Vector2 origin = new Vector2(transform.position.x + direction.x * screenWidthHalf, transform.position.y + direction.y * screenHeightHalf);
        Vector2 size = Vector2.zero;

        if (direction.x != 0)
        {
            size.x = cameraCollisionLookahead;
            size.y = 2 * screenHeightHalf - 2;
        }
        else if (direction.y != 0)
        {
            size.x = 2 * screenWidthHalf - 2;
            size.y = cameraCollisionLookahead;
        }

        return Physics2D.BoxCast(origin, size, 0, direction, cameraCollisionLookahead, LayerMask.GetMask("LevelBoundary"));
    }

    //
    // Set the x position target for the camera to move towards.
    //
    // @param playerPos
    //  Current player position in vector.
    //
    // @param newPlayerDirection
    //  Direction the player is facing.
    //
    // @param moving
    //  Whether or not the player is moving fast enough for the camera to follow.
    //
    public void SetHorizontalTarget(Vector2 playerPos, float newPlayerDirection, bool moving)
    {
        cameraTarget.x = playerPos.x + newPlayerDirection * forwardDistance;

        playerIsMoving = moving;
        
        if (newPlayerDirection != playerDirection)
        {
            cameraTurnTimer = cameraTurnTime;
        }

        playerDirection = newPlayerDirection;
        playerPosition = playerPos;
    }

    //
    // Set the horizontal speed of the player.
    //
    // @param speed
    //  Player's horizontal speed.
    //
    public void SetPlayerHorizontalSpeed(float speed)
    {
        playerHorizontalSpeed = speed;
    }

    public void SetVerticalSpeed(float speed)
    {
        cameraVerticalSpeed = speed;
    }

    public void SetVerticalTarget(Vector2 playerPos, float delay)
    {
        if (cameraTarget.y != playerPos.y)
        {
            cameraTarget.y = playerPos.y;
        }
    }

    public void SetVerticalOffset(float offset)
    {
        verticalOffset = offset;
    }

    public VerticalState GetVerticalState()
    {
        return verticalState;
    }

    public void SetVerticalState(VerticalState state)
    {
        verticalState = state;
    }
}
