﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public float speed = 20f;

    private Rigidbody2D rb2d;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = transform.up * speed;
    }

    void Update()
    {
        if (!PointIsVisibleToCamera(rb2d.position))
        {
            gameObject.SetActive(false);
        }
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