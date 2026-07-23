using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSwitchEventScriptableObject", menuName = "Scriptable Objects/WeaponSwitchEventChanelScriptableObject")]
public class WeaponSwitchEventChanelScriptableObject : ScriptableObject
{
    /// <summary>
    /// list of listeners
    /// </summary>
    private List<Weapon> listeners;

    /// <summary>
    /// add listener who is going to handle the event
    /// </summary>
    /// <param name="listener"></param>
    public void AddListeners(Weapon listener)
    {
        listeners.Add(listener);
    }

    /// <summary>
    /// remove a listener
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveListeners(Weapon listener)
    {
        listeners.Remove(listener);
    }

    /// <summary>
    /// method to log all the listener for a event
    /// used of debbugging puporses
    /// </summary>
    public void GetListeners()
    {
        foreach (Weapon l in listeners)
        {
            Debug.Log($"listener owner {l.OwnerClientId}");
        }
    }

    /// <summary>
    /// enable of disable gameobject depending on the given parameter
    /// </summary>
    /// <param name="isGameobjectActive"></param>
    public void SetIsGameobjectActive(ulong clientId, string objectToEnable)
    {
        foreach (Weapon l in listeners)
        {
            l.SetIsGameobjectActive(clientId, objectToEnable);
        }
    }

}
