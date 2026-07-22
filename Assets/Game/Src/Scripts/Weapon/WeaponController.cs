using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WeaponController : NetworkBehaviour
{
    public GameObject[] weapons;
    private GameObject currentWeapon; // état local, pas de NetworkVariable
    public NetworkVariable<int> weaponIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); 

    [SerializeField]
    private Transform weaponHolder;

    /// <summary>
    /// event chanel where event related to weapon switch will be published
    /// </summary>
    [SerializeField]
    private WeaponSwitchEventChanelScriptableObject weaponSwitchEventChanel;
    public override void OnNetworkSpawn()
    {

        base.OnNetworkSpawn();
        Debug.LogWarning($"WeaponController spawned for client {OwnerClientId}. Weapons list size : {weapons.Length} Current weapon index: {weaponIndex.Value}");
    
        weaponIndex.OnValueChanged += SwitchWeapon;
        SwitchWeapon(0, 1);
      //  DisableUnequipedWeapon(weaponIndex.Value);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        weaponIndex.OnValueChanged -= SwitchWeapon;
    }

    public void SwitchWeapon(int previousValue, int newValue)
    {
        int index = IsValidWeaponIndex(newValue) ? newValue : previousValue;
        int previousIndex = IsValidWeaponIndex(previousValue) ? previousValue : 0;
        Debug.LogWarning($"Client {OwnerClientId} requested weapon change to index {newValue}");
        RequestWeaponChangeServerRpc(OwnerClientId, previousIndex, index);
    }

    /// <summary>
    /// select the next weapon in the array, by calculating the next index and sending a request to the server to change the weapon.
    /// </summary>
    public void SelectNextWeaponIndex()
    {
        weaponIndex.Value = weaponIndex.Value == weapons.Length - 1 ? 0 : weaponIndex.Value + 1;
    }
    /// <summary>
    /// execute the weapon change on the server, validate the request and update the state, then notify all clients of the change.
    /// </summary>
    /// <param name="requestedIndex"></param>
    /// <param name="rpcParams"></param>
    [Rpc(SendTo.Server)]
    private void RequestWeaponChangeServerRpc(ulong clientId, int previousValue, int requestedIndex)
    {
        Debug.LogWarning($"Client {OwnerClientId} requested weapon change to index {requestedIndex}");
        currentWeapon = weapons[requestedIndex];
        Debug.Log($"Client  {OwnerClientId} requested {weapons[requestedIndex]} current weapon {currentWeapon}");
        UpdateWeaponClientRpc(clientId, requestedIndex);

    }

    /// <summary>
    /// send a request to the server to fire
    /// </summary>
    public void RequestWeaponFire()
    {
        currentWeapon.TryGetComponent<Weapon>(out Weapon weaponComponent);
        Debug.Log($"RequestWeaponFire weapon component {weaponComponent} {weaponComponent.NetworkBehaviourId} {weaponComponent.IsSpawned}");
        weaponComponent.RequestWeaponFireServerRpc();
       
    }
    /// <summary>
    /// send a request to the server to reload the weapon
    /// </summary>
    public void RequestWeaponReload()
    {
        currentWeapon.TryGetComponent<Weapon>(out Weapon weaponComponent);
        Debug.Log($"RequestWeaponReload weapon component {weaponComponent}");
        weaponComponent?.RequestReloadServerRpc();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="previousIndex"></param>
    /// <param name="currentIndex"></param>

    [Rpc(SendTo.Everyone)]
    private void UpdateWeaponClientRpc(ulong clientId, int requestedIndex)
    {
        // set currentWeapon here because the client cannot execute the server rpc function
        // so for the client who aren't hosting the game the currentWeapon will not be set
        currentWeapon = weapons[requestedIndex];
        DisableUnequipedWeapon(clientId, currentWeapon.name);
        Debug.Log($"Weapon changed  to {currentWeapon.name}");
    }

    /// <summary>
    /// disable weapon that name is different from the equiped weapon name
    /// </summary>
    /// <param name="equipedWeaponName"></param>
    private void DisableUnequipedWeapon(ulong clientId, string equipedWeaponName)
    {
        for (int i = 0; i < weapons.Length; i++) {
            weaponSwitchEventChanel.SetIsGameobjectActive(clientId, equipedWeaponName);
        }
    }

    /// <summary>
    /// validate the index throught the weapons array length, return true if the index is valid, false otherwise.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool IsValidWeaponIndex(int index)
    {
        return index >= 0 && index < weapons.Length;
    }
}
