using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public BoxCollider2D hitBox;

    bool spawned = false;

    Patrol patrol;

    // Start is called before the first frame update
    void Start()
    {
        patrol = GetComponent<Patrol>();
        hitBox.edgeRadius = 0.01f;
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
        
        spawned = true;
    }
}
