using UnityEngine;

public class ProjectileAttack : EnemyAttack
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;

    public override void HandleAttack()
    {
        if (player == null || projectilePrefab == null) return;

        float distance = Vector2.Distance(enemy.transform.position, player.position);
        if ( Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            // جهت حرکت به سمت player
            Vector3 dir = (player.position - enemy.transform.position).normalized;

            // اسپان پرتابه
            GameObject proj = GameObject.Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);

            // تنظیم چرخش پرتابه
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // اگر Rigidbody2D دارد، سرعت بده
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * projectileSpeed;
            }

            Debug.Log($"{enemy.name} shoots projectile -> direction: {dir}");
        }
    }
}