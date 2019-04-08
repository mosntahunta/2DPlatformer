﻿using System.Collections;
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
    public Text scoreText;
    public Text coinsText;
    public Text currentLevelText;
    public Text timeText;
    
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
