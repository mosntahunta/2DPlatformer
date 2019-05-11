using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minimumHorizontalSpeed = 0.5f;
    public float slowHorizontalSpeed = 2f;
    public float fastHorizontalSpeed = 10f;

    public float forwardDistance = 1f;
    public float slowdownFactor = 5f;
    public float cameraDrag = 0.4f;

    private float cameraVerticalSpeed = 3f;

    private float cameraHorizontalSpeed = 0f;
    private float playerHorizontalSpeed = 0f;

    private float playerDirection = 0f;
    private bool playerIsMoving = false;
    
    public float cameraStartUpTime = 0.5f;
    private float cameraStartUpTimer = 0.0f;

    private bool cameraFromBehind = false;
    private bool snap = false;

    private float groundPositionY = -4.5f; // placeholder variable for now

    Vector2 playerPosition;
    Vector3 cameraTarget;

    HorizontalState horizontalState;
    enum HorizontalState
    {
        SlowMoving,
        FastMoving
    }

    VerticalState verticalState;
    public enum VerticalState
    {
        GroundLock,
        PlatformLock,
        TopLock,
        FreeMoving
    }


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;
        horizontalState = HorizontalState.SlowMoving;
        verticalState = VerticalState.FreeMoving;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        HorizontalMovement();
        VerticalMovement();
    }

    void HorizontalMovement()
    {
        float cameraToTarget = Mathf.Abs(cameraTarget.x - transform.position.x);
        float playerToTarget = Mathf.Abs(cameraTarget.x - playerPosition.x);

        if (horizontalState == HorizontalState.FastMoving)
        {
            if (cameraToTarget < playerToTarget)
            {
                cameraHorizontalSpeed = Mathf.Lerp(slowHorizontalSpeed / slowdownFactor, playerHorizontalSpeed, cameraToTarget / playerToTarget);
            }
            else if (cameraToTarget > playerToTarget)
            {
                cameraFromBehind = true;
                cameraHorizontalSpeed = playerIsMoving ? fastHorizontalSpeed : playerHorizontalSpeed;
            }
            else
            {
                cameraHorizontalSpeed = playerHorizontalSpeed;
            }

            if (!playerIsMoving && cameraFromBehind)
            {
                cameraHorizontalSpeed = Mathf.Lerp(cameraHorizontalSpeed, 0.0f, cameraDrag);
            }
        }
        else
        {
            if (cameraToTarget > playerToTarget)
            {
                cameraHorizontalSpeed = slowHorizontalSpeed;
            }
            else
            {
                cameraHorizontalSpeed = Mathf.Lerp(slowHorizontalSpeed / slowdownFactor, slowHorizontalSpeed, cameraToTarget / playerToTarget);
            }
        }

        // limit the camera speed
        if (cameraHorizontalSpeed < minimumHorizontalSpeed) cameraHorizontalSpeed = minimumHorizontalSpeed;

        // turning ramp up time
        if (cameraStartUpTimer > 0)
        {
            cameraStartUpTimer -= Time.deltaTime;
            cameraHorizontalSpeed = Mathf.Lerp(cameraHorizontalSpeed, 0.0f, cameraStartUpTimer / cameraStartUpTime);
        }

        Vector3 horizontalTarget = new Vector3(cameraTarget.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, Time.deltaTime * cameraHorizontalSpeed);

        if (Mathf.Abs(cameraTarget.x - transform.position.x) == 0.0f)
        {
            horizontalState = HorizontalState.SlowMoving;
        }
    }

    void VerticalMovement()
    {
        if (verticalState == VerticalState.FreeMoving)
        {
            transform.position = new Vector3(transform.position.x, cameraTarget.y, transform.position.z);
        }
        else if (verticalState == VerticalState.PlatformLock)
        {
            Vector3 verticalTarget = new Vector3(transform.position.x, cameraTarget.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, verticalTarget, Time.deltaTime * cameraVerticalSpeed);
        }
        
    }

    void SnapToPosition(float yPos)
    {
        snap = true;
    }

    public void SetHorizontalTarget(Vector2 playerPos, float direction, bool isMaxSpeed)
    {
        cameraTarget.x = playerPos.x + direction * forwardDistance;
        
        if (isMaxSpeed)
        {
            if (horizontalState == HorizontalState.SlowMoving)
            {
                cameraStartUpTimer = 0.35f;
            }
            horizontalState = HorizontalState.FastMoving;
        }
        else if (direction != playerDirection)
        {
            horizontalState = HorizontalState.SlowMoving;
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

        if (!playerIsMoving)
        {
            cameraFromBehind = false;
        }
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

    public VerticalState GetVerticalState()
    {
        return verticalState;
    }

    public void SetVerticalState(VerticalState state)
    {
        verticalState = state;
    }
}
