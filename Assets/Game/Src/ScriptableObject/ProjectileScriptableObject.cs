using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "Scriptable Objects/ProjectileScriptableObject")]
public class ProjectileScriptableObject : ScriptableObject
{
    public string projectileName;
    public int damage;
    public GameObject prefab;
}
