using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDammagable
{
    private const float k_DefaultHealth = 100.0f;
    /// <summary>
    /// the player's health
    /// </summary>
    private NetworkVariable<float> health = new NetworkVariable<float>(k_DefaultHealth, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    /// <summary>
    /// chanel event for updating the local UI
    /// </summary>
    [SerializeField] LocalUIEventChanelScriptableObject eventChanelScriptableObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }

    /// <summary>
    /// decrease the health value , then raise an event in the event chanel
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        health.Value = Mathf.Clamp(health.Value - damage, 0, k_DefaultHealth);
        eventChanelScriptableObject.RaiseUpdateHealthUIEvent(ToPercentage(health.Value, k_DefaultHealth));
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
