using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 10f;

    [Header("Hit VFX")]
    public GameObject hitVFX;
    public float vfxLifeTime = 1.5f;

    [Header("Lifetime")]
    public float maxLifetime = 5f;

    protected bool hasHit;

    protected virtual void OnEnable()
    {
        hasHit = false;
        Invoke(nameof(DestroySelf), maxLifetime);
    }

    protected virtual void OnDisable()
    {
        CancelInvoke();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Debug.Log("Projectile hit PLAYER ✅");
            health.TakeDamage(damage);
            OnHit();
            return;
        }

        // برخورد با زمین / دیوار و...
        OnHit();
    }

    protected virtual void OnHit()
    {
        Debug.Log("ProjectileBase.OnHit called ✅");

        hasHit = true;
        SpawnHitVFX();
        DestroySelf();
    }

    protected virtual void SpawnHitVFX()
    {
        if (hitVFX == null) return;

        GameObject vfx = Instantiate(
            hitVFX,
            transform.position,
            Quaternion.identity
        );

        Destroy(vfx, vfxLifeTime);
    }

    protected virtual void DestroySelf()
    {
        Destroy(gameObject);
        // Pool → gameObject.SetActive(false);
    }
}