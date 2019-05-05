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

        if (horizontalMoveState == HorizontalMoveState.Fast)
        {
            float playerToTarget = Mathf.Abs(cameraTarget.x - playerPosition.x);
            
            if (cameraToTarget < slowdownDistance)
            {
                currentCameraSpeed = Mathf.Lerp(playerHorizontalSpeed / slowdownFactor, playerHorizontalSpeed, cameraToTarget / slowdownDistance);
            }
            else if (cameraToTarget > playerToTarget)
            {
                float cameraToPlayer = Mathf.Abs(playerPosition.x - transform.position.x);
                currentCameraSpeed = Mathf.Lerp(playerHorizontalSpeed, fastHorizontalSpeed, cameraToPlayer / forwardDistance);
            }
            else
            {
                currentCameraSpeed = Mathf.Lerp(slowHorizontalSpeed, playerHorizontalSpeed, cameraToTarget / playerToTarget);
            }
        }
        else
        {
            if (cameraToTarget > slowdownDistance)
            {
                currentCameraSpeed = slowHorizontalSpeed;
            }
            else
            {
                currentCameraSpeed = Mathf.Lerp(slowHorizontalSpeed / slowdownFactor, slowHorizontalSpeed, cameraToTarget / slowdownDistance);
            }
        }

        if (cameraStartUpTimer > 0)
        {
            cameraStartUpTimer -= Time.deltaTime;
            currentCameraSpeed = Mathf.Lerp(currentCameraSpeed, 0.0f, cameraStartUpTimer / cameraStartUpTime);
        }

        if (currentCameraSpeed < minimumHorizontalSpeed) currentCameraSpeed = minimumHorizontalSpeed;

        transform.position = Vector3.MoveTowards(transform.position, cameraTarget, Time.deltaTime * currentCameraSpeed);

        if (Mathf.Abs(cameraTarget.x - transform.position.x) < slowdownDistance)
        {
            horizontalMoveState = HorizontalMoveState.Slow;
        }
    }

    void SnapToPosition(float yPos)
    {
        snap = true;
    }

    public void SetHorizontalTarget(float targetXPos, Vector2 currentPos, float direction, bool isMaxSpeed)
    {
        cameraTarget = transform.position;
        cameraTarget.x = targetXPos + direction * forwardDistance;
        
        if (isMaxSpeed)
        {
            horizontalMoveState = HorizontalMoveState.Fast;
        }
        else if (direction != playerDirection)
        {
            cameraStartUpTimer = cameraStartUpTime;
        }

        playerDirection = direction;
        playerPosition = currentPos;
    }

    public void SetPlayerHorizontalSpeed(float speed)
    {
        playerHorizontalSpeed = speed;
    }
}
