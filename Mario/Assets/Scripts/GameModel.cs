using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameModel : MonoBehaviour
{
    public static GameModel SharedInstance;
    public PlayerData playerData;

    [Serializable]
    public struct PlayerData
    {
        public int currentLives;
        public int maxLives;
        public int currentLevelIndex;
    }

    void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(gameObject);

            SharedInstance = this;

            SharedInstance.playerData.currentLives = playerData.currentLives;
            SharedInstance.playerData.maxLives = playerData.maxLives;
            SharedInstance.playerData.currentLevelIndex = playerData.currentLevelIndex;
        }
        else if (SharedInstance != this)
        {
            Destroy(gameObject);
        }
    }

    // save data out to a file
    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        
        bf.Serialize(file, playerData);
        file.Close();
    }

    // load data from a saved file
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            playerData.currentLives = data.currentLives;
            playerData.maxLives = data.maxLives;
            playerData.currentLevelIndex = data.currentLevelIndex;
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadYourAsyncScene(playerData.currentLevelIndex));
    }

    public void PrepareForLoadingSavedLevel()
    {
        LoadData();
    }

    public void PrepareForTransitionToNextLevel()
    {
        playerData.currentLevelIndex++;
        SaveData();
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
