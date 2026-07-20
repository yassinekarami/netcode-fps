using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "Scriptable Objects/ProjectileScriptableObject")]
public class ProjectileScriptableObject : ScriptableObject
{
    public string projectileName;
    public int damage;
    public int speed;
    public int ttl = 10;
    public GameObject vfx;
    public GameObject realProjectile;
    public GameObject dummyProjectile;
}
