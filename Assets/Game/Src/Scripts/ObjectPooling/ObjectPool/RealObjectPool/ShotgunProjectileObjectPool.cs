using UnityEngine;

public class ShotgunProjectileObjectPool : ObjectPool<ShotgunProjectilePooledObject>
{
    public void Initialize(string path, int size, Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>(path);

        for (int i = 0; i < size; i++)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            instance.SetActive(false);

            ShotgunProjectilePooledObject projectile =
                instance.GetComponent<ShotgunProjectilePooledObject>();

            objectPool.Enqueue(projectile);
        }
    }
}
