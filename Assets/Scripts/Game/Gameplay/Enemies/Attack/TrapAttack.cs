using UnityEngine;

public class TrapAttack : EnemyAttack
{
    [Header("Trap Settings")]
    public GameObject trapPrefab;
    public float trapOffset = 1.5f;

    public override void HandleAttack()
    {
        if (trapPrefab == null || player == null) return;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            Vector3 dropPos = player.position + Vector3.down * trapOffset;

            GameObject.Instantiate(trapPrefab, dropPos, Quaternion.identity);
            Debug.Log($"{enemy.name} places a TRAP near player!");
        }
    }
}