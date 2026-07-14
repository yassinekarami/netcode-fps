using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectPool<T> where T: AbstractPooledObject
{

    protected List<T> spawnedObjects = new List<T>();

    protected Queue<T> objectPool = new Queue<T>();

    /// <summary>
    /// Spawn an object at a position then added to the swpan object queue and remove it from the pool
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="ownerId"></param>
    public void SpawnObject(Vector3 position, Quaternion rotation, ulong ownerId)
    {
    
        if(objectPool.Count == 0)
        {
            Debug.LogWarning("Object pool is empty, cannot spawn object.");
            return;
        }
        // when an object is spawned, it is removed from the queue and added to the list of spawned objects
        Debug.Log($"object has been spawn at position {position} owned by {ownerId} remaining object in the pool {objectPool.Count}");
        T obj = objectPool.Dequeue();

        Debug.Log($"obj {obj.name} ");

        obj.transform.position = position;
        obj.transform.rotation = rotation;

        obj.gameObject.SetActive(true);

        spawnedObjects.Add(obj);

        Debug.Log($"Spawned object. Remaining: {objectPool.Count}");
    }


    /// <summary>
    /// Despawn an object by making it inactive then added it in the object pool
    /// </summary>
    /// <param name="obj"></param>
    public void Despawn(T obj)
    {
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj);
    }

}
