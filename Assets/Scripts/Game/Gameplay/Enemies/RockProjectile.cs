using UnityEngine;

public class RockProjectile : ProjectileBase
{
    private Vector2 direction;
    private float speed;
    private bool isActive;

    protected override void OnEnable()
    {
        base.OnEnable();
        isActive = false;
    }

    private void Update()
    {
        if (!isActive || hasHit) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void Setup(Vector2 dir, float moveSpeed)
    {
        direction = dir.normalized;
        speed = moveSpeed;
        isActive = true;
    }

    // ✅ هیچ افکتی اینجا نداریم
    protected override void OnHit()
    {
        Debug.Log("RockProjectile.OnHit 🔥");
        base.OnHit();
    }
}