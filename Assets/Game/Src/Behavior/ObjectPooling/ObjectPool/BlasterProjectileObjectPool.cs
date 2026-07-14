using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections.Generic;

public class BlasterProctileObjectPool: ObjectPool<BlasterProjectilePooledObject>
{
    public void Initialize(int size)
    {
        GameObject prefab = Resources.Load<GameObject>(
            "Prefabs/Projectiles/Real/real_projectile_Blaster"
        );

        for (int i = 0; i < size; i++)
        {
            GameObject instance = Object.Instantiate(prefab);
            instance.SetActive(false);

            BlasterProjectilePooledObject projectile =
                instance.GetComponent<BlasterProjectilePooledObject>();

            objectPool.Enqueue(projectile);
        }
    }

}
