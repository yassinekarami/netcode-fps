using Unity.Netcode;
using UnityEngine;


public abstract class AbstractPooledObject: NetworkBehaviour
{
    public abstract void SpawnObject();


    public abstract void DespawnObject();
}
