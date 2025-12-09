using UnityEngine;

public class AreaAttack : EnemyAttack
{
    public float effectRadius = 2.5f;

    public override void HandleAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(enemy.transform.position, effectRadius);
        foreach (var hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"{enemy.name} triggers AREA attack!");
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}