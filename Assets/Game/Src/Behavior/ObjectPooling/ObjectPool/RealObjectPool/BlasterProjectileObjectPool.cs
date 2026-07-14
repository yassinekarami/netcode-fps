using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class BlasterProctileObjectPool: ObjectPool<BlasterProjectilePooledObject>
{

    public void Initialize(string path, int size, Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>(path);

        for (int i = 0; i < size; i++)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            instance.SetActive(false);

            BlasterProjectilePooledObject projectile =
                instance.GetComponent<BlasterProjectilePooledObject>();

            objectPool.Enqueue(projectile);
        }
    }

}
