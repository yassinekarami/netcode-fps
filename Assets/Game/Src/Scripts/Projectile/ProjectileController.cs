using Unity.Netcode;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
    [SerializeField]
    private ProjectileScriptableObject data;

    private float timer = 0f;

    private void FixedUpdate()
    {
        transform.position += transform.forward * data.speed * Time.fixedDeltaTime;
        timer += Time.fixedDeltaTime;
        if (timer > data.ttl)
        {
            ObjectPoolManager.instance.DespawnObject(data.projectileName, gameObject.GetComponent<BlasterProjectilePooledObject>());
            timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        other.gameObject.TryGetComponent<IDammagable>(out IDammagable component);
        component?.TakeDamage(data.damage);


    }
}
