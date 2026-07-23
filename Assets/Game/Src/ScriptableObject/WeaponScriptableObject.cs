using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/WeaponScriptableObject")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public ProjectileScriptableObject projectileData;
    public float fireRate;
    public int maxMagazine;
    public int magazineSize;
    public Sprite icon;
}
