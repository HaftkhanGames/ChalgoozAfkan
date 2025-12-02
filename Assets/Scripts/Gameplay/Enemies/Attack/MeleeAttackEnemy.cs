using UnityEngine;
using System.Collections;

public class MeleeAttack : EnemyAttack
{
    [Header("Collider Settings")]
    public Collider2D attackCollider;
    public float activeDuration = 0.2f;

    public override void HandleAttack()
    {
        if (player == null || attackCollider == null) return;

        float distance = Vector2.Distance(enemy.transform.position, player.position);
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(ActivateCollider());
        }
    }

    private IEnumerator ActivateCollider()
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider.enabled && other.CompareTag("Player"))
        {
            // other.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            Debug.Log($"{enemy.name} hit {other.name} with Melee Collider.");
        }
    }
}