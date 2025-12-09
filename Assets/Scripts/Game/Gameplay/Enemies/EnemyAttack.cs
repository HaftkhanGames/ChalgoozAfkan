using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    // public float attackRange = 3f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;

    protected EnemyBase enemy;
    protected Transform player;
    protected float nextAttackTime;

    public virtual void Initialize(EnemyBase baseEnemy)
    {
        enemy = baseEnemy;
        player = baseEnemy.player;
    }

    /// <summary>
    /// اجرای رفتار حمله (هر نوع زیرکلاس خودش override می‌کند)
    /// </summary>
    public abstract void HandleAttack();
}