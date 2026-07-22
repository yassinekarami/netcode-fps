using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Data structure for holding the weaponData
/// </summary>
struct weaponDataStruct
{
    public int currentAmmo { get; set; }
    public int magazineSize { get; set; }
    public int maxMagazine { get; set; }

    public bool hasBeenInitialized { get; set; }

}
public class Weapon : NetworkBehaviour
{
    /// <summary>
    /// list of gameobject childs to enable or disable
    /// </summary>
    [SerializeField]
    private GameObject[] componentToDisabledEnable;
    /// <summary>
    /// event chanel where event related to UI ( firing / reloading ... ) will be published
    /// </summary>
    [SerializeField]
    private LocalUIEventChanelScriptableObject localUIEventChanel;
    /// <summary>
    /// event chanel where event related to weapon switch will be published
    /// </summary>
    [SerializeField]
    private WeaponSwitchEventChanelScriptableObject weaponSwitchEventChanel;
    /// <summary>
    /// scriptable object holding the initial weapon data
    /// </summary>
    [SerializeField]
    private WeaponScriptableObject weaponData;
    /// <summary>
    /// structure holding a copy of the weapon data for the weapon's firing / reloading logic
    /// </summary>
    weaponDataStruct weaponAmmo;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"weapon has been spawned {gameObject.name} {NetworkBehaviourId}");
        weaponSwitchEventChanel.AddListeners(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        weaponSwitchEventChanel.RemoveListeners(this);
    }
    private void OnEnable()
    {
        if (!weaponAmmo.hasBeenInitialized)
        {
            weaponAmmo.currentAmmo = weaponData.magazineSize;
            weaponAmmo.maxMagazine = weaponData.maxMagazine;
            weaponAmmo.magazineSize = weaponData.magazineSize;
            weaponAmmo.hasBeenInitialized = true;
        }

        localUIEventChanel.RaiseUpdateAmmoUIEvent(ToPercentage(weaponAmmo.currentAmmo, weaponAmmo.magazineSize));
        localUIEventChanel.RaiseUpdateMagazineNumberUIEvent(weaponAmmo.maxMagazine);

    }

    public void SetIsGameobjectActive(ulong clientId, string objectToEnable)
    {
        if (clientId != OwnerClientId) return;
        Debug.Log($"gameobject to enable {objectToEnable} current gameobject name {gameObject.name}");
        if  (gameObject.name == objectToEnable)
        {
            foreach (GameObject go in componentToDisabledEnable)
            {
                go.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject go in componentToDisabledEnable)
            {
                go.SetActive(false);
            }
        }
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
        if (weaponAmmo.currentAmmo > 0)
        {
            weaponAmmo.currentAmmo--;
            localUIEventChanel.RaiseUpdateAmmoUIEvent(ToPercentage(weaponAmmo.currentAmmo, weaponAmmo.magazineSize));
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
        DummyObjectPoolManager.instance.SpawnObject(weaponData.projectileData.projectileName, transform.position, transform.rotation, OwnerClientId);
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
     
        bool hasReloadingSucceed = ReloadWeapon();
        if (hasReloadingSucceed) 
        {
            localUIEventChanel.RaiseUpdateAmmoUIEvent(ToPercentage(weaponAmmo.currentAmmo, weaponAmmo.magazineSize));
            localUIEventChanel.RaiseUpdateMagazineNumberUIEvent(weaponAmmo.maxMagazine);
        }

        Debug.Log($"Reloaded weapon: {weaponData.weaponName}. Current ammo: {weaponAmmo.currentAmmo}");
    }

    /// <summary>
    /// reload the weapon by resetting the current ammo to the magazine size and reducing the max ammo accordingly.
    /// </summary>
    private bool ReloadWeapon()
    {
        if (weaponAmmo.maxMagazine > 0)
        {
            weaponAmmo.currentAmmo = weaponData.magazineSize;
            weaponAmmo.maxMagazine--;
            return true;
        }
        return false;
    }

    /// <summary>
    /// return currentValue percentage of maxValue
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    private float ToPercentage(float currentValue, float maxValue)
    {
        return currentValue / maxValue;
    }

}
