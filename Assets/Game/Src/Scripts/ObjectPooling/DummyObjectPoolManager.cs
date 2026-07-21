using Unity.Netcode;
using UnityEngine;

public class DummyObjectPoolManager : NetworkBehaviour
{
    public static DummyObjectPoolManager instance { get; private set;  }
    
    public static DummyBlasterProjectileObjectPool dummyBlasterProjectileObjectPool { get; private set; }

    public static DummyShotgunProjectileObjectPool dummyShotgunProjectileObjectPool { get; private set; }
    public static DummyDiscProjectileObjectPool dummyDiscProjectileObjectPool { get; private set; }
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
            }
            if (dummyShotgunProjectileObjectPool == null)
            {
                dummyShotgunProjectileObjectPool = new DummyShotgunProjectileObjectPool();
                dummyShotgunProjectileObjectPool.Initialize("Prefabs/Projectiles/Dummy/dummy_projectile_Shotgun", 3, instance.gameObject.transform);
            }

            if (dummyDiscProjectileObjectPool == null)
            {

                dummyDiscProjectileObjectPool = new DummyDiscProjectileObjectPool();
                dummyDiscProjectileObjectPool.Initialize("Prefabs/Projectiles/Dummy/dummy_projectile_Disc", 3, instance.gameObject.transform);
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
            dummyShotgunProjectileObjectPool.SpawnObject(position, rotation, ownerId);
        }
        else if (name == "DiscProjectile")
        {
          //  dummyDiscProjectileObjectPool.SpawnObject(position, rotation, ownerId);
            Debug.LogWarning("disc projectile is not available");
        }
    }
}
