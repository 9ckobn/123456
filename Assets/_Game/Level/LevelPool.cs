using System.Collections.Generic;
using UnityEngine;

public class LevelPool
{
    private Dictionary<int, Queue<GameObject>> poolDictionary;

    public LevelPool()
    {
        poolDictionary = new Dictionary<int, Queue<GameObject>>();
    }

    public void CreatePool(GameObject prefab, int size)
    {
        int key = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(key))
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary[key] = objectPool;
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int key = prefab.GetInstanceID();

        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning("Pool with key " + key + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[key].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[key].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}