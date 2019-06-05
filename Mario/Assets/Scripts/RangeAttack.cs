using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : Attack
{
    public float speed = 20f;
    public float range = 10f;

    private float startX = 0f;

    private Rigidbody2D rb2d;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = transform.up * speed;
        startX = rb2d.position.x;
    }

    void Update()
    {
        if (!PointIsVisibleToCamera(rb2d.position) || Mathf.Abs(rb2d.position.x - startX) >= range)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        gameObject.SetActive(false);
    }

    private bool PointIsVisibleToCamera(Vector2 point)
    {
        if (Camera.main.WorldToViewportPoint(point).x < 0 ||
            Camera.main.WorldToViewportPoint(point).x > 1 ||
            Camera.main.WorldToViewportPoint(point).y > 1 ||
            Camera.main.WorldToViewportPoint(point).y < 0)
        {
            return false;
        }
        return true;
    }
}
