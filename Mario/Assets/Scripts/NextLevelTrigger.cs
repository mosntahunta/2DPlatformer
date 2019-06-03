using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        // todo - this is temporary for testing current level
        if (collider.gameObject.tag == "Player")
        {
            SceneController.SharedInstance.ReloadCurrentScene();
        }
    }
}
