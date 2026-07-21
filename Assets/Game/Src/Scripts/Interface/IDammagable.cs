using UnityEngine;

public interface IDammagable
{
    /// <summary>
    /// apply damages
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage);
}
