using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PointDisplay : MonoBehaviour
{
    public Camera mainCamera;
    public Font arcadeFont;

    public float scoreTextAnimationTime = 2f;
    public float scoreTextAnimationValue = 100f;

    public void DisplayScoreAtPosition(int score, Vector2 position)
    {
        // Create the score display text game object
        GameObject gameObject = new GameObject("ScoreDisplayText");
        foreach (Transform childTransform in GameControl.control.transform)
        {
            if (childTransform.tag == "Canvas")
            {
                gameObject.transform.SetParent(childTransform);
            }
        }
        
        // Set the score display text
        Text scoreDisplayText = gameObject.AddComponent<Text>();
        scoreDisplayText.text = score.ToString();
        scoreDisplayText.font = arcadeFont;
        scoreDisplayText.alignment = TextAnchor.UpperCenter;

        // Set the initial position of the text in viewport coordiantes
        Vector2 viewportPoint = mainCamera.WorldToViewportPoint(position);
        RectTransform rectTransform = scoreDisplayText.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;

        // Start the text moving animation
        StartCoroutine(AnimateScoreText(scoreDisplayText));
    }

    IEnumerator AnimateScoreText(Text text)
    {
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        Vector2 currentPosition = rectTransform.localPosition;
        Vector2 destination = new Vector2(rectTransform.localPosition.x, rectTransform.localPosition.y + scoreTextAnimationValue);

        float t = 0f;
        while (t < 1.0f)
        {
            if (rectTransform != null)
            {
                t += Time.deltaTime / scoreTextAnimationTime;
                rectTransform.localPosition = Vector2.Lerp(currentPosition, destination, t);
                yield return null;
            }
            else
            {
                break;
            }
        }

        Destroy(text.gameObject);
    }
}
