using Unity.Netcode;
using UnityEngine;


public class Weapon : NetworkBehaviour
{
    /// <summary>
    /// list of gameobject childs to enable or disable
    /// </summary>
    [SerializeField]
    private GameObject[] componentToDisabledEnable;
    /// <summary>
    /// event chanel where event related to UI will be published
    /// </summary>
    [SerializeField]
    private LocalUIEventChanelScriptableObject localUIEventChanel;
    /// <summary>
    /// event chanel where event related to weapon switch will be published
    /// </summary>
    [SerializeField]
    private WeaponSwitchEventChanelScriptableObject networkWeaponSwitchEventChanel;
    /// <summary>
    /// scriptable object holding the initial weapon data
    /// </summary>
    [SerializeField]
    private WeaponScriptableObject weaponDataSO;
    /// <summary>
    /// network variable for synchronizing the current ammo
    /// </summary>
    public NetworkVariable<int> currentAmmo = new NetworkVariable<int>(default, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);
    /// <summary>
    /// network variable for synchronizing the remaining magazine
    /// </summary>
    public NetworkVariable<int> remainingMagazine = new NetworkVariable<int>(default, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

    private bool hasBeenInitiated = false;

    /// <summary>
    /// called when the network object is spawned , initialize the variable, and add event when the values changes
    /// </summary>
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkWeaponSwitchEventChanel.AddListeners(this);
        if (IsOwner)
        {
            Debug.Log($"weapon has been spawned {gameObject.name} {NetworkBehaviourId}");

        }
        if (IsServer)
        {
            if (!hasBeenInitiated)
            {
                currentAmmo.Value = weaponDataSO.magazineSize;
                remainingMagazine.Value = weaponDataSO.maxMagazine;
                hasBeenInitiated = true;
            }
        }

        currentAmmo.OnValueChanged += HandleUpdateAmmoUI;
        remainingMagazine.OnValueChanged += HandleUpdateRemainingMagazineUI;
        HandleUpdateWeaponIconUI(weaponDataSO.icon);
    }

    /// <summary>
    /// called when the network object is despawn
    /// </summary>
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        networkWeaponSwitchEventChanel.RemoveListeners(this);
        currentAmmo.OnValueChanged -= HandleUpdateAmmoUI;
        remainingMagazine.OnValueChanged -= HandleUpdateRemainingMagazineUI;
    }

    private void OnEnable()
    {
        HandleUpdateAmmoUI(-1, currentAmmo.Value);
        HandleUpdateRemainingMagazineUI(-1, remainingMagazine.Value);
    }

    /// <summary>
    /// Enables this object's configured GameObjects if its name matches the
    /// specified target; otherwise, disables them.
    /// </summary>
    /// <param name="clientId">The client requesting the update.</param>
    /// <param name="objectToEnable">The name of the GameObject to enable.</param>
    public void SetIsGameobjectActive(ulong clientId, string objectToEnable)
    {
        if (clientId != OwnerClientId) return;
        Debug.Log($"gameobject to enable {objectToEnable} current gameobject name {gameObject.name}");
        if  (gameObject.name == objectToEnable)
        {
            foreach (GameObject go in componentToDisabledEnable)
            {
                go.SetActive(true);
                HandleUpdateAmmoUI(-1, currentAmmo.Value);
                HandleUpdateRemainingMagazineUI(-1, remainingMagazine.Value);
                HandleUpdateWeaponIconUI(weaponDataSO.icon);
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
    /// raise an event to update the remaining ammo UI
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="nextValue"></param>
    private void HandleUpdateAmmoUI(int previousValue, int nextValue)
    {
        if (!IsOwner) { return; }
        localUIEventChanel.RaiseUpdateAmmoUIEvent(ToPercentage(nextValue, weaponDataSO.magazineSize));
    }


    /// <summary>
    /// raise an event to update the remaining magazine UI
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="nextValue"></param>
    private void HandleUpdateRemainingMagazineUI(int previousValue, int nextValue)
    {
        if (!IsOwner) { return; }
        localUIEventChanel.RaiseUpdateMagazineNumberUIEvent(nextValue);
    }

    private void HandleUpdateWeaponIconUI(Sprite weaponIcon)
    {
        if (!IsOwner) { return; }
        localUIEventChanel.RaiseUpdateWeaponIconEvent(weaponIcon);
    }

    /// <summary>
    /// send a request to the server to fire the weapon
    /// </summary>
    /// <param name="rpcParams"></param>
    [Rpc(SendTo.Server)]
    public void RequestWeaponFireServerRpc()
    {
        Debug.Log($"Client {OwnerClientId} requested weaponFire");

        if (currentAmmo == null || remainingMagazine == null)
        {
            Debug.LogWarning("Weapon data is not assigned.");
            return;
        }
        // Logique de tir ici, par exemple instancier un projectile
        Debug.Log($"Firing weapon: {weaponDataSO.weaponName}");
        if (currentAmmo.Value > 0)
        {
            currentAmmo.Value--;
            ObjectPoolManager.instance.SpawnObject(weaponDataSO.projectileData.projectileName, transform.position, transform.rotation, OwnerClientId);
            ApplyWeaponFireVisualClientRpc();
        }
        else
        {
            Debug.Log("Out of ammo , Reloading!");
            Reload();
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
        DummyObjectPoolManager.instance.SpawnObject(weaponDataSO.projectileData.projectileName, transform.position, transform.rotation, OwnerClientId);
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
        if (currentAmmo == null || remainingMagazine == null)
        {
            Debug.LogWarning("Weapon data is not assigned.");
            return;
        }
        Reload();



    }
    /// <summary>
    /// function called to reload the weapon 
    /// it called either when the player request a reload or when the player try to shoot and there is currentAmmo = 0
    /// </summary>
    private void Reload()
    {
        if (remainingMagazine.Value > 0)
        {
            currentAmmo.Value = weaponDataSO.magazineSize;
            remainingMagazine.Value--;
        }
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
