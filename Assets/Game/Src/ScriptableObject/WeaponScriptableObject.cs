using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/WeaponScriptableObject")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public ProjectileScriptableObject projectileData;
    public int index;
}
