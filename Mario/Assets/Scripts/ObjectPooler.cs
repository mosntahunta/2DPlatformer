using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool = 0;
    public bool shouldExpand = false;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<ObjectPoolItem> itemsToPool;
    public List<GameObject> pooledObjects;

    void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(gameObject);

            SharedInstance = this;
        }
        else if (SharedInstance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();

        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                CreateAndAddGameObject(ref item.objectToPool);
            }
        }
    }

    private GameObject CreateAndAddGameObject(ref GameObject objectToPool)
    {
        GameObject obj = Instantiate(objectToPool);
        obj.SetActive(false);
        pooledObjects.Add(obj);

        // todo - bullets should persist across different levels, but later if you have screens (e.g. menu)
        // that don't require bullets, should we delete and recreate when another level is loaded?
        DontDestroyOnLoad(obj); 
        return obj;
    }

    //
    // Search for a pooled object that is not currently active in the scene and matches the given tag
    //
    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }

        // Create a new game object if expanding the pool
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    return CreateAndAddGameObject(ref item.objectToPool);
                }
            }
        }

        return null;
    }
}
