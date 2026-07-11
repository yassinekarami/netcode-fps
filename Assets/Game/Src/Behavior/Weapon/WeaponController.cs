using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;


public class WeaponController : NetworkBehaviour
{
    public GameObject[] weapons;
    WeaponScriptableObject[] allWeapons;
    private NetworkVariable<int> networkWeaponId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        allWeapons = Resources.LoadAll<WeaponScriptableObject>("Weapon");

        // load all weapons from resources folder
        networkWeaponId.OnValueChanged += SelectNextWeapon;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsOwner) return;

        networkWeaponId.OnValueChanged -= SelectNextWeapon;
    }

    private void SelectNextWeapon(int previousValue, int newValue)
    {
        Debug.Log($"Weapon changed from {previousValue} to {newValue}");
    
        if(previousValue == 0)
        {
            weapons[0].SetActive(false);
        }
        else
        {
            weapons[previousValue - 1].SetActive(false);
        }
        if (newValue >= weapons.Length)
        {
            weapons[0].SetActive(true);
        }
        else
        {
            weapons[newValue].SetActive(true);
        }
    }

    public void SelectNextWeapon()
    {
        if (!IsOwner) return;
        networkWeaponId.Value = networkWeaponId.Value == allWeapons.Length ? 1 : networkWeaponId.Value + 1;
    }
}
