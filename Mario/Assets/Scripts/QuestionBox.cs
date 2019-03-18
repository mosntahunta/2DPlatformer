﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    [SerializeField] Mushroom mushroomPrefab; 
    Animator animator;
    GameObject player;
    
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == player.gameObject.tag && !animator.GetBool("Dying"))
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 center = collision.collider.bounds.center;

            float minX = collision.collider.bounds.min.x;
            float maxX = collision.collider.bounds.max.x;

            if (contactPoint.y > center.y && contactPoint.x > minX && contactPoint.x < maxX)
            {
                animator.SetBool("Dying", true);

                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                Mushroom mushroom = Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
                Vector2 destination = new Vector2(mushroom.transform.position.x, mushroom.transform.position.y + renderer.bounds.size.y);
                IEnumerator coroutine = mushroom.SpawnToPosition(mushroom.transform, destination, 1.5f);
                StartCoroutine(coroutine);
            }
        }
    }
}
