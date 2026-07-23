using Unity.Netcode;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    /// <summary>
    /// array containing all the weapons
    /// </summary>
    public GameObject[] weapons;

    /// <summary>
    /// network variable holding the index of the current equiped weapon
    /// </summary>
    public NetworkVariable<int> weaponIndex = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); 

    /// <summary>
    /// tranform of the weapon parent
    /// </summary>
    [SerializeField]
    private Transform weaponHolder;

    /// <summary>
    /// event chanel where event related to weapon switch will be published
    /// </summary>
    [SerializeField]
    private WeaponSwitchEventChanelScriptableObject networkWeaponSwitchEventChanel;

    /// <summary>
    /// Registers weapon change events when the object is spawned.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        weaponIndex.OnValueChanged += SwitchWeapon;
   
        if (!IsOwner) return;
        Debug.Log($"WeaponController spawned for client {OwnerClientId}. Weapons list size : {weapons.Length} Current weapon index: {weaponIndex.Value}");
        RequestWeaponChangeServerRpc();

    }

    /// <summary>
    /// Unregisters weapon change events when the object is despawned.
    /// </summary>
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        weaponIndex.OnValueChanged -= SwitchWeapon;
    }

    /// <summary>
    /// execute the weapon change on the server, validate the request and update the state, then notify all clients of the change.
    /// </summary>
    /// <param name="requestedIndex"></param>
    /// <param name="rpcParams"></param>
    [Rpc(SendTo.Server)]
    public void RequestWeaponChangeServerRpc()
    {
        int nextIndex = SelectNextWeaponIndex();
        weaponIndex.Value = IsValidWeaponIndex(nextIndex) ? nextIndex : 0;
        Debug.LogWarning($"Client {OwnerClientId} requested weapon change to index {weaponIndex.Value}");
        Debug.Log($"Client  {OwnerClientId} requested {weapons[weaponIndex.Value]} current weapon {weapons[weaponIndex.Value]}");
      
    }

    /// <summary>
    /// send a request to the server to fire
    /// </summary>
    public void RequestWeaponFire()
    {
        weapons[weaponIndex.Value].TryGetComponent<Weapon>(out Weapon weaponComponent);
        Debug.Log($"RequestWeaponFire weapon component {weaponComponent} {weaponComponent.NetworkBehaviourId} {weaponComponent.IsSpawned}");
        weaponComponent.RequestWeaponFireServerRpc();
       
    }
    /// <summary>
    /// send a request to the server to reload the weapon
    /// </summary>
    public void RequestWeaponReload()
    {
        weapons[weaponIndex.Value].TryGetComponent<Weapon>(out Weapon weaponComponent);
        Debug.Log($"RequestWeaponReload weapon component {weaponComponent}");
        weaponComponent?.RequestReloadServerRpc();
    }

    /// <summary>
    /// disable weapon that name is different from the equiped weapon name
    /// </summary>
    /// <param name="equipedWeaponName"></param>
    private void DisableUnequipedWeapon(ulong clientId, string equipedWeaponName)
    {
        for (int i = 0; i < weapons.Length; i++) {
            networkWeaponSwitchEventChanel.SetIsGameobjectActive(clientId, equipedWeaponName);
        }
    }

    /// <summary>
    /// Requests a weapon change on the server.
    /// </summary>
    /// <param name="previousValue">The previous weapon index.</param>
    /// <param name="newValue">The new weapon index.</param>
    public void SwitchWeapon(int previousValue, int newValue)
    {
        int newIndex = IsValidWeaponIndex(newValue) ? newValue : previousValue;
        int previousIndex = IsValidWeaponIndex(previousValue) ? previousValue : 0;
        Debug.LogWarning($"Client {OwnerClientId} requested weapon change to index {newValue}");
        DisableUnequipedWeapon(OwnerClientId, weapons[newIndex].name);
    }

    /// <summary>
    /// select the next weapon in the array, by calculating the next index and sending a request to the server to change the weapon.
    /// </summary>
    private int SelectNextWeaponIndex()
    {
        return weaponIndex.Value == weapons.Length - 1 ? 0 : weaponIndex.Value + 1;
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
