using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public GameObject destroyedBricks;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 center = collision.collider.bounds.center;

            float minX = collision.collider.bounds.min.x;
            float maxX = collision.collider.bounds.max.x;

            if (contactPoint.y > center.y && contactPoint.x > minX && contactPoint.x < maxX && player.marioType != Player.Type.CHILD)
            {
                GameObject obj = Instantiate(destroyedBricks, transform.position, transform.rotation);
                Destroy(gameObject);
                Destroy(obj, 2);
            }
        }
    }
}
