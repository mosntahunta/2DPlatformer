using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class PickupItem : MonoBehaviour
{
    // the item id in the `ItemDatabase`
    public int itemId;
}
