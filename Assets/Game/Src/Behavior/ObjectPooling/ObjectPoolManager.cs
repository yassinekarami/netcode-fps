using Unity.Netcode;
using UnityEngine;

public class ObjectPoolManager : NetworkBehaviour
{
    public static ObjectPoolManager instance { get; private set; }

    public static BlasterProctileObjectPool blasterProjecileObjectPool { get; private set; }
    public override void OnNetworkSpawn()
    {
        Debug.Log("ObjectPoolManager spawned");
        base.OnNetworkSpawn();
        if (!IsHost) return;
        if (instance == null) instance = this;
 
        blasterProjecileObjectPool = new BlasterProctileObjectPool();
        blasterProjecileObjectPool.Initialize(10, instance.gameObject.transform);

    }


    public void SpawnObject(string name, Vector3 position, Quaternion rotation, ulong ownerId)
    {
        if (name == "BlasterProjectile")
        {
            blasterProjecileObjectPool.SpawnObject(position, rotation, ownerId);
        }
    }

}
