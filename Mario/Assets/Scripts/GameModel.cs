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
    public GameData gameData;

    [Serializable]
    public struct PlayerData
    {
        public int currentLives;
        public int maxLives;
    }

    [Serializable]
    public struct GameData
    {
        public int currentSceneIndex;
    }

    void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(gameObject);

            SharedInstance = this;

            SharedInstance.playerData.currentLives = playerData.currentLives;
            SharedInstance.playerData.maxLives = playerData.maxLives;

            SharedInstance.gameData.currentSceneIndex = gameData.currentSceneIndex;
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

        CreateSaveFile<PlayerData>("/playerInfo.dat", playerData, bf);
        CreateSaveFile<GameData>("/gameInfo.dat", gameData, bf);
    }

    private void CreateSaveFile<T>(string fileName, T data, BinaryFormatter bf )
    {
        FileStream file = File.Create(Application.persistentDataPath + fileName);
        bf.Serialize(file, data);
        file.Close();
    }

    // load data from a saved file
    public void LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            playerData.currentLives = data.currentLives;
            playerData.maxLives = data.maxLives;
        }

        if (File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            gameData.currentSceneIndex = data.currentSceneIndex;
        }
    }
}
