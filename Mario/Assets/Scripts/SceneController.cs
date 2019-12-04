using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log("Scene Index: " + scene.buildIndex);

        // todo - this is temporary for now until we have proper level loading later
        //GameModel.SharedInstance.LoadData();

        //GameModel.SharedInstance.gameData.currentSceneIndex = scene.buildIndex;
    }

    public void ReloadCurrentScene()
    {
        //SceneManager.LoadScene(GameModel.SharedInstance.gameData.currentSceneIndex);
    }
}
