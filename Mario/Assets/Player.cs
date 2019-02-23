using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField] float horizontalSpeed = 5.0f;

    Rigidbody2D myRigidBody2D;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float direction = CrossPlatformInputManager.GetAxis("Horizontal");
        myRigidBody2D.velocity = new Vector2(direction * horizontalSpeed, 0.0f);
    }
}
