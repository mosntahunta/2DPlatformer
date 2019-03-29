using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public float itemSpawnTime = 1.5f;
    public Mushroom mushroomPrefab;
    public Flower flowerPrefab;
    public Coin coinPrefab;
    public ItemType itemType;

    Animator animator;
    GameObject player;

    public enum ItemType
    {
        MUSHROOM,
        FLOWER,
        COIN
    }

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

                switch (itemType)
                {
                    case ItemType.MUSHROOM:
                    {
                        Mushroom mushroom = Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
                        Vector2 destination = new Vector2(mushroom.transform.position.x, mushroom.transform.position.y + renderer.bounds.size.y);
                        IEnumerator coroutine = mushroom.SpawnToPosition(mushroom.transform, destination, itemSpawnTime);
                        StartCoroutine(coroutine);
                    }
                    break;
                    case ItemType.FLOWER:
                    {
                        Flower flower = Instantiate(flowerPrefab, transform.position, Quaternion.identity);
                        Vector2 destination = new Vector2(flower.transform.position.x, flower.transform.position.y + renderer.bounds.size.y);
                        IEnumerator coroutine = flower.SpawnToPosition(flower.transform, destination, itemSpawnTime);
                        StartCoroutine(coroutine);
                    }
                    break;
                    case ItemType.COIN:
                    {
                        Coin coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
                        coin.KickUp(new Vector2(0f, 24f));
                    }
                    break;
                }
            }
        }
    }
}
