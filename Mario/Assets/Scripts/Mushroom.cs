using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Vector2 startPosition;
    Vector2 targetPosition;
    float timeToReachTarget;
    float interpolationFactor = 0;
    bool spawned = false;

    Patrol patrol;

    // Start is called before the first frame update
    void Start()
    {
        patrol = GetComponent<Patrol>();
    }

    // Update is called once per frame
    void Update()
    {
        // only start patrolling after the mushroom has fully spawned
        if (spawned)
        {
            patrol.Proceed();
        }
    }

    public IEnumerator SpawnToPosition(Transform transform, Vector2 destination, float timeToReachTarget)
    {
        Vector2 currentPos = transform.position;
        float t = 0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector2.Lerp(currentPos, destination, t);
            yield return null;
        }
        
        spawned = true;
        yield return null;
    }
}
