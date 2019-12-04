using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameModel : PersistableObject
{
    [SerializeField] KeyCode saveKey = KeyCode.K;
    [SerializeField] KeyCode loadKey = KeyCode.L;

    [SerializeField] PersistentStorage storage;
    [SerializeField] PlayerModel playerModel;
    [SerializeField] InventoryModel inventoryModel;

    private int loadedLevelBuildIndex;

    private const int saveVersion = 1;
    
    void Start()
    {
        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedLevel = SceneManager.GetSceneAt(i);
                if (loadedLevel.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedLevel);
                    loadedLevelBuildIndex = loadedLevel.buildIndex;
                    Debug.Log("loaded level index: " + loadedLevelBuildIndex);
                    return;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            storage.Load(this);
        }
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        // you can also show loading screen at this point
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(loadedLevelBuildIndex);
        playerModel.Save(writer);
        inventoryModel.Save(writer);
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        yield return LoadLevel(reader.ReadInt());
        playerModel.Load(reader);
        inventoryModel.Load(reader);
    }
}
