using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalUIEventChanelScriptableObject", menuName = "Scriptable Objects/LocalUIEventChanelScriptableObject")]
public class LocalUIEventChanelScriptableObject : ScriptableObject
{
    /// <summary>
    /// list of listeners
    /// </summary>
    private List <LocalUIController> listeners;

    /// <summary>
    /// add listener who is going to handle the event
    /// </summary>
    /// <param name="listener"></param>
    public void AddListeners(LocalUIController listener)
    {
        listeners.Add(listener);
    }

    /// <summary>
    /// remove a listner
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveListeners(LocalUIController listener) { 
        listeners.Remove(listener);
    }

    /// <summary>
    /// make the listener update the health UI
    /// </summary>
    /// <param name="health"></param>
    public void RaiseUpdateHealthUIEvent(float health)
    {
        foreach(LocalUIController l in listeners )
        {
            l.UpdateHealthUI(health);
        }
    }

    /// <summary>
    /// make the listener update the ammo UI ( used when firing )
    /// </summary>
    /// <param name="number"></param>
    public void RaiseUpdateAmmoUIEvent(float number)
    {
        foreach (LocalUIController l in listeners)
        {
            l.UpdateAmmoUIEvent(number);
        }
    }

    /// <summary>
    /// make the listener update the remaning magazine number ( when reloading )
    /// </summary>
    /// <param name="number"></param>
    public void RaiseUpdateMagazineNumberUIEvent(int number)
    {
        foreach (LocalUIController l in listeners)
        {
            l.UpdateMagazineNumberUI(number);
        }
    }

    /// <summary>
    /// make the listener update the whole ammo UI ( when changing weapon )
    /// </summary>
    /// <param name="number"></param>
    public void RaiseUpdateWeaponUIEvent(int number)
    {
        foreach (LocalUIController l in listeners)
        {
            l.UpdateWeaponUI(number);
        }
    }
}
