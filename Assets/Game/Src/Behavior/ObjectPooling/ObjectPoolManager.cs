using Unity.Netcode;
using UnityEngine;

public class ObjectPoolManager : NetworkBehaviour
{
    public static ObjectPoolManager instance { get; private set; }

    public static BlasterProctileObjectPool blasterProjecileObjectPool { get; private set; }
    public static ShotgunProjectileObjectPool shotgunProjecileObjectPool { get; private set; }
    public static DiscProjectileObjectPool discProjecileObjectPool { get; private set; }
    public override void OnNetworkSpawn()
    {
        Debug.Log("ObjectPoolManager spawned");
        base.OnNetworkSpawn();
        if (!IsHost) return;
        if (instance == null) instance = this;
 
        blasterProjecileObjectPool = new BlasterProctileObjectPool();
        blasterProjecileObjectPool.Initialize("Prefabs/Projectiles/Real/real_projectile_Blaster", 10, instance.gameObject.transform);

        shotgunProjecileObjectPool = new ShotgunProjectileObjectPool();
        shotgunProjecileObjectPool.Initialize("Prefabs/Projectiles/Real/real_projectile_Shotgun", 10, instance.gameObject.transform);

        discProjecileObjectPool = new DiscProjectileObjectPool();
        discProjecileObjectPool.Initialize("Prefabs/Projectiles/Real/real_projectile_Disc", 10, instance.gameObject.transform);
    }


    public void SpawnObject(string name, Vector3 position, Quaternion rotation, ulong ownerId)
    {
        if (name == "BlasterProjectile")
        {
            blasterProjecileObjectPool.SpawnObject(position, rotation, ownerId);
        }
        else if (name == "ShotgunProjectile")
        {
            shotgunProjecileObjectPool.SpawnObject(position, rotation, ownerId);
        }
        else if(name == "DiscProjectile")
        {
            discProjecileObjectPool.SpawnObject(position, rotation, ownerId);
        }
    }

}
