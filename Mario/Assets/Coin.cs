﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int score = 200;
    private bool canBePickedUp = true;

    public void KickUp(Vector2 upVelocity)
    {
        gameObject.layer = LayerMask.NameToLayer("Transparent");
        canBePickedUp = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = upVelocity;

        int currentCoins = GameControl.control.getCoins();
        if (currentCoins < 100)
        {
            GameControl.control.setCoins(currentCoins + 1);
        }
        GameControl.control.setScore(GameControl.control.getScore() + score);

        PointDisplay pointDisplay = GameObject.FindGameObjectWithTag("PointDisplay").GetComponent<PointDisplay>();
        if (pointDisplay)
        {
            pointDisplay.DisplayScoreAtPosition(score, transform.position);
        }

        Destroy(gameObject, 0.45f);
    }

    public bool CanBePickedUp()
    {
        return canBePickedUp;
    }
}