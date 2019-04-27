using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaPlant : MonoBehaviour
{
    public float transitionDuration = 1.0f;

    Vector2 start;
    Vector2 destination;
    float expandHeight = 0.0f;
    float transitionTimer = 0.0f;
    bool expanded = false;
    
    void Start()
    {
        expandHeight = GetComponent<BoxCollider2D>().bounds.size.y;
        start = new Vector2(transform.position.x, transform.position.y);
        destination = new Vector2(transform.position.x, transform.position.y + expandHeight);
    }
    
    void Update()
    {
        if (!expanded)
        {
            if (transitionTimer < 1.0f)
            {
                Vector2 currentPos = transform.position;
                transitionTimer += Time.deltaTime / transitionDuration;
                transform.position = Vector2.Lerp(currentPos, destination, transitionTimer);
            }
            else
            {
                transitionTimer = 0.0f;
                expanded = true;
            }
        }
        else if (expanded)
        {
            if (transitionTimer < 1.0f)
            {
                Vector2 currentPos = transform.position;
                transitionTimer += Time.deltaTime / transitionDuration;
                transform.position = Vector2.Lerp(currentPos, start, transitionTimer);
            }
            else
            {
                transitionTimer = 0.0f;
                expanded = false;
            }
        }
    }
}
