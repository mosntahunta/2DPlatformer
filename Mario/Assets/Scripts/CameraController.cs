using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minimumHorizontalSpeed = 0.5f;
    public float slowHorizontalSpeed = 2f;
    public float fastHorizontalSpeed = 10f;

    public float forwardDistance = 1f;
    public float slowdownDistance = 0.5f;
    public float slowdownFactor = 5f;

    private float currentCameraSpeed = 0f;
    private float playerHorizontalSpeed = 0f;

    private float playerDirection = 0f;
    private bool playerIsMoving = false;
    
    public float cameraStartUpTime = 0.5f;
    private float cameraStartUpTimer = 0.0f;

    private bool snap = false;

    Vector2 playerPosition;
    Vector3 cameraTarget;

    HorizontalMoveState horizontalMoveState;
    enum HorizontalMoveState
    {
        Slow,
        Fast
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;
        horizontalMoveState = HorizontalMoveState.Slow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        float cameraToTarget = Mathf.Abs(cameraTarget.x - transform.position.x);
        float playerToTarget = Mathf.Abs(cameraTarget.x - playerPosition.x);

        if (horizontalMoveState == HorizontalMoveState.Fast)
        {
            if (cameraToTarget < playerToTarget - 0.1f)
            {
                currentCameraSpeed = Mathf.Lerp(slowHorizontalSpeed / slowdownFactor, playerHorizontalSpeed, cameraToTarget / (playerToTarget - 0.1f));
            }
            else if (cameraToTarget > playerToTarget + 0.1f)
            {
                if (playerIsMoving)
                {
                    currentCameraSpeed = fastHorizontalSpeed;
                }
                else
                {
                    currentCameraSpeed = playerHorizontalSpeed;
                }
            }
            else
            {
                currentCameraSpeed = playerHorizontalSpeed;
            }
        }
        else
        {
            if (cameraToTarget > playerToTarget)
            {
                currentCameraSpeed = slowHorizontalSpeed;
            }
            else
            {
                currentCameraSpeed = Mathf.Lerp(slowHorizontalSpeed / slowdownFactor, slowHorizontalSpeed, cameraToTarget / playerToTarget);
            }
        }

        // limit the camera speed
        if (currentCameraSpeed < minimumHorizontalSpeed) currentCameraSpeed = minimumHorizontalSpeed;

        // turning ramp up time
        if (cameraStartUpTimer > 0)
        {
            cameraStartUpTimer -= Time.deltaTime;
            currentCameraSpeed = Mathf.Lerp(currentCameraSpeed, 0.0f, cameraStartUpTimer / cameraStartUpTime);
        }

        // apply speed to position
        transform.position = Vector3.MoveTowards(transform.position, cameraTarget, Time.deltaTime * currentCameraSpeed);

        if (Mathf.Abs(cameraTarget.x - transform.position.x) == 0.0f)
        {
            horizontalMoveState = HorizontalMoveState.Slow;
        }
    }

    void SnapToPosition(float yPos)
    {
        snap = true;
    }

    public void SetHorizontalTarget(Vector2 playerPos, float direction, bool isMaxSpeed)
    {
        cameraTarget = transform.position;
        cameraTarget.x = playerPos.x + direction * forwardDistance;
        
        if (isMaxSpeed)
        {
            horizontalMoveState = HorizontalMoveState.Fast;
        }
        else if (direction != playerDirection)
        {
            horizontalMoveState = HorizontalMoveState.Slow;
            cameraStartUpTimer = cameraStartUpTime;
        }

        playerDirection = direction;
        playerPosition = playerPos;
    }

    public void SetPlayerHorizontalSpeed(float speed)
    {
        playerHorizontalSpeed = speed;
    }
    
    public void SetPlayerIsMoving(bool moving)
    {
        playerIsMoving = moving;
    }
}
