using Unity.Netcode;
using UnityEngine;

public class Server : NetworkBehaviour
{
    [SerializeField]
    WeaponScriptableObject[] allWeapons;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsHost) return;

        allWeapons = Resources.LoadAll<WeaponScriptableObject>("Weapon");
    }
}
