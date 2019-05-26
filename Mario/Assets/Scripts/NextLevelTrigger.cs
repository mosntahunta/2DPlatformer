using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && collider.GetType() == typeof(CapsuleCollider2D))
        {
            //GameControl.control.PrepareForTransitionToNextLevel();
        }
    }
}
