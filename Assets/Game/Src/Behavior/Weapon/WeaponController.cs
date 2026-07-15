using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    public GameObject[] weapons;
    private GameObject currentWeapon; // état local, pas de NetworkVariable
    private int currentWeaponIndex = 0; // état local, pas de NetworkVariable

    [SerializeField]
    private Transform weaponHolder;

    public override void OnNetworkSpawn()
    {

        base.OnNetworkSpawn();
        Debug.LogWarning($"WeaponController spawned for client {OwnerClientId}. Weapons list size : {weapons.Length} Current weapon index: {currentWeaponIndex}");
        // Applique l'état initial pour tout le monde, y compris les late-joiners

        currentWeapon = weapons[currentWeaponIndex];
        DisableUnequipedWeapon(currentWeaponIndex);
        ApplyWeaponVisual(-1, currentWeaponIndex);
    }

    /// <summary>
    /// select the next weapon in the array, by calculating the next index and sending a request to the server to change the weapon.
    /// </summary>
    public void SelectNextWeapon()
    {
        int nextIndex = currentWeaponIndex == weapons.Length - 1 ? 0 : currentWeaponIndex + 1;
        RequestWeaponChangeServerRpc(currentWeaponIndex, nextIndex);
    }
    /// <summary>
    /// execute the weapon change on the server, validate the request and update the state, then notify all clients of the change.
    /// </summary>
    /// <param name="requestedIndex"></param>
    /// <param name="rpcParams"></param>
    [Rpc(SendTo.Server)]
    private void RequestWeaponChangeServerRpc(int previousIndex, int requestedIndex)
    {
        if (!IsValidWeaponIndex(requestedIndex))
        {
            Debug.LogWarning($"Client {OwnerClientId} requested invalid weapon index {requestedIndex}");
            return;
        }

        Debug.LogWarning($"Client {OwnerClientId} requested weapon change to index {requestedIndex}");
        currentWeaponIndex = requestedIndex;
        currentWeapon = weapons[requestedIndex];
        UpdateWeaponClientRpc(previousIndex, currentWeaponIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RequestWeaponFire()
    {
        currentWeapon.TryGetComponent<Weapon>(out var weaponComponent);
        Debug.Log($"weapon component {weaponComponent}");
        weaponComponent?.RequestWeaponFireServerRpc();
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="previousIndex"></param>
    /// <param name="newIndex"></param>

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateWeaponClientRpc(int previousIndex, int newIndex)
    {
        currentWeaponIndex = newIndex;
        ApplyWeaponVisual(previousIndex, newIndex);
    }
    /// <summary>
    /// apply the weapon visual change by deactivating the previous weapon and activating the new one, with debug logs for tracking.
    /// </summary>
    /// <param name="previousIndex"></param>
    /// <param name="newIndex"></param>
    private void ApplyWeaponVisual(int previousIndex, int newIndex)
    {
        Debug.Log($"Weapon changed from {previousIndex} to {newIndex}");

        if (IsValidWeaponIndex(previousIndex))
        {
            weapons[previousIndex].SetActive(false);
        }
        

        if (IsValidWeaponIndex(newIndex))
        {
            weapons[newIndex].SetActive(true);
        }
         
        else
        {
            Debug.LogWarning($"Invalid weapon index: {newIndex}");
        }
    }
    /// <summary>
    /// disable weapon that index is different from the equiped weapon index
    /// </summary>
    /// <param name="equipedWeaponsIndex"></param>
    private void DisableUnequipedWeapon(int equipedWeaponsIndex)
    {
        for (int i = 0; i < weapons.Length; i++) {
            if (i != equipedWeaponsIndex)
            {
                weapons[i].SetActive(false);
            }
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
