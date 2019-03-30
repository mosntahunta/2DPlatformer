using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour
{
    public static GameControl control;
    public Camera mainCamera;
    public Canvas canvasGameObject;
    public Text scoreText;
    public Text coinsText;
    public Text currentLevelText;
    public Text timeText;
    public Font arcadeFont;

    public float scoreTextAnimationTime = 2f;
    public float scoreTextAnimationValue = 100f;

    private int lives = 3;
    private int coins = 0;
    private int score = 0;
    private int time = 360;
    private string currentLevel = "WORLD\n1-1";

    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;

            control.setLives(lives);
            control.setCoins(coins);
            control.setScore(score);
            control.setTime(time);
            control.setCurrentLevel(currentLevel);
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }

    // save data out to a file
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData(lives, coins, score, currentLevel);
        bf.Serialize(file, data);
        file.Close();
    }

    public void setLives(int lives)
    {
        this.lives = lives;
    }

    public int getLives()
    {
        return this.lives;
    }

    public void setCoins(int coins)
    {
        this.coins = coins;

        coinsText.text = "\n" + "X" + coins.ToString();
    }

    public int getCoins()
    {
        return this.coins;
    }

    public void setScore(int score)
    {
        this.score = score;

        scoreText.text = "MARIO" + "\n" + score.ToString();
    }

    public int getScore()
    {
        return this.score;
    }

    public void setTime(int time)
    {
        this.time = time;

        timeText.text = "Time" + "\n" + time.ToString();
    }

    public int getTime()
    {
        return this.time;
    }

    public void setCurrentLevel(string currentLevel)
    {
        this.currentLevel = currentLevel;

        currentLevelText.text = currentLevel.ToString();
    }

    public string getCurrentLevel()
    {
        return this.currentLevel;
    }

    public void DisplayScoreAtPosition(int score, Vector2 position)
    {
        // Create the score display text game object
        GameObject gameObject = new GameObject("ScoreDisplayText");
        gameObject.transform.SetParent(canvasGameObject.transform);

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
                print(rectTransform.localPosition.y);
                yield return null;
            }
            else
            {
                break;
            }
        }

        Destroy(text.gameObject);
    }

    // load data from a saved file
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            setLives(data.lives);
            setCoins(data.coins);
            setScore(data.score);
            setCurrentLevel(data.currentLevel);
        }
    }

    public void LoadNextScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        StartCoroutine(LoadYourAsyncScene(scene.buildIndex + 1));
    }

    IEnumerator LoadYourAsyncScene(int sceneIndex)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}

[Serializable]
class PlayerData
{
    public PlayerData(int lives, int coins, int score, string currentLevel)
    {
        this.lives = lives;
        this.coins = coins;
        this.score = score;
        this.currentLevel = currentLevel;
    }

    public int lives;
    public int coins;
    public int score;
    public string currentLevel;
}
