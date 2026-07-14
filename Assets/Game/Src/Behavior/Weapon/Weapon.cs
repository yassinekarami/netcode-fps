using System;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{

    [SerializeField]
    private WeaponScriptableObject weaponData;

    private int currentAmmo;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentAmmo = weaponData.magazineSize;
    }

    /// <summary>
    /// send a request to the server to fire the weapon
    /// </summary>
    /// <param name="rpcParams"></param>
    [Rpc(SendTo.Server)]
    public void RequestWeaponFireServerRpc()
    {
        
        Debug.Log($"Client {OwnerClientId} requested weaponFire");

        if (weaponData == null)
        {
            Debug.LogWarning("Weapon data is not assigned.");
            return;
        }
        // Logique de tir ici, par exemple instancier un projectile
        Debug.Log($"Firing weapon: {weaponData.weaponName}");
        if (currentAmmo > 0)
        {
            currentAmmo--;
            ObjectPoolManager.instance.SpawnObject(weaponData.projectileData.projectileName, transform.position, transform.rotation, OwnerClientId);
            ApplyWeaponFireVisualClientRpc();
        }
        else
        {
            Debug.Log("Out of ammo , Reloading!");
            RequestReloadServerRpc();
        }
    }

    /// <summary>
    /// apply the visual effect of firing the weapon on all clients except the owner who is the authority 
    /// the host display real projectile while the clients display dummy projectile
    /// </summary>
    [Rpc(SendTo.NotAuthority)]
    public void ApplyWeaponFireVisualClientRpc()
    {
        Debug.Log($"Client {OwnerClientId} applying weapon fire visual effect");
        Instantiate(weaponData.projectileData.dummyProjectile, transform.position, transform.rotation);

    }

    /// <summary>
    /// send a request to the server to reload the weapon
    /// </summary>
    /// <param name="rpcParams"></param>
    [Rpc(SendTo.Server)]
    public void RequestReloadServerRpc()
    {
        Debug.LogWarning($"Client {OwnerClientId} requested reload");
        if (!IsOwner) return;
        if (weaponData == null)
        {
            Debug.LogWarning("Weapon data is not assigned.");
            return;
        }
     
        ReloadWeapon();
        Debug.Log($"Reloaded weapon: {weaponData.weaponName}. Current ammo: {currentAmmo}");
    }

    /// <summary>
    /// reload the weapon by resetting the current ammo to the magazine size and reducing the max ammo accordingly.
    /// </summary>
    private void ReloadWeapon()
    {
        // Logique de rechargement ici
        currentAmmo = weaponData.magazineSize;
        weaponData.maxAmmo -= weaponData.magazineSize;
    }

}
