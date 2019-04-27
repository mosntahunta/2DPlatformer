using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public IEnumerator SpawnToPosition(Transform transform, Vector2 destination, float timeToReachTarget)
    {
        Vector2 currentPos = transform.position;
        float t = 0f;
        while (t < 1.0f)
        {
            if (transform != null)
            {
                t += Time.deltaTime / timeToReachTarget;
                transform.position = Vector2.Lerp(currentPos, destination, t);
                yield return null;
            }
            else
            {
                break;
            }
        }
    }
}
