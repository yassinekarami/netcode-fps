using Unity.Netcode;
using UnityEngine;

public class DummyObjectPoolManager : NetworkBehaviour
{
    public static DummyObjectPoolManager instance { get; private set;  }
    
    public static DummyBlasterProjectileObjectPool dummyBlasterProjectileObjectPool { get; private set; }

    public static DummyShotgunProjectileObjectPool dummyShotgunProjecileObjectPool { get; private set; }
    public static DummyDiscProjectileObjectPool dummyDiscProjecileObjectPool { get; private set; }
    public override void OnNetworkSpawn()
    {
        Debug.Log("DummyObjectPoolManager spawned");
        base.OnNetworkSpawn();
        if (IsHost)
        {
            return;
        }
        else
        {
            if (instance == null) instance = this;
            if (dummyBlasterProjectileObjectPool == null)
            {
                dummyBlasterProjectileObjectPool = new DummyBlasterProjectileObjectPool();
                dummyBlasterProjectileObjectPool.Initialize("Prefabs/Projectiles/Dummy/dummy_projectile_Blaster", 3, instance.gameObject.transform);


                dummyShotgunProjecileObjectPool = new DummyShotgunProjectileObjectPool();
                dummyShotgunProjecileObjectPool.Initialize("Prefabs/Projectiles/Dummy/dummy_projectile_Shotgun", 3, instance.gameObject.transform);

                dummyDiscProjecileObjectPool = new DummyDiscProjectileObjectPool();
                dummyDiscProjecileObjectPool.Initialize("Prefabs/Projectiles/Dummy/dummy_projectile_Disc", 3, instance.gameObject.transform);
            }
        }
    }


    public void SpawnObject(string name, Vector3 position, Quaternion rotation, ulong ownerId)
    {
        if (name == "BlasterProjectile")
        {
            dummyBlasterProjectileObjectPool.SpawnObject(position, rotation, ownerId);
        }
        else if (name == "ShotgunProjectile")
        {
            dummyShotgunProjecileObjectPool.SpawnObject(position, rotation, ownerId);
        }
        else if (name == "DiscProjectile")
        {
            dummyDiscProjecileObjectPool.SpawnObject(position, rotation, ownerId);
        }
    }
}
