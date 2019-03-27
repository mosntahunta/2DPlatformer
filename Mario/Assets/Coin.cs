using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool canBePickedUp = true;

    public IEnumerator SpawnToPosition(Transform transform, Vector2 destination, float timeToReachTarget)
    {
        canBePickedUp = false;
        Vector2 currentPos = transform.position;
        Vector2 startingPos = currentPos;
        float t = 0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector2.Lerp(currentPos, destination, t);
            yield return null;
        }

        currentPos = transform.position;
        t = 0f;

        // go back down halfway
        while (t < 0.4f)
        {
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector2.Lerp(currentPos, startingPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.02f);

        Destroy(gameObject);
    }

    public bool CanBePickedUp()
    {
        return canBePickedUp;
    }
}
