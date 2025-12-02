using UnityEngine;

public abstract class EnemyMobility : MonoBehaviour
{
    [Header("Mobility Settings")]
    public float moveSpeed = 2f;
    public float orbitRadius = 2f;
    public float waveAmplitude = 1f;
    public float waveFrequency = 1f;
    public float teleportCooldown = 3f;

    protected EnemyBase enemy; // ارجاع به EnemyBase
    protected Transform player;
    protected Vector3 startPos;

    public virtual void Initialize(EnemyBase baseEnemy)
    {
        enemy = baseEnemy;
        player = baseEnemy.player;
        startPos = enemy.transform.position;
    }

    /// <summary>
    /// اجرای رفتار حرکتی (هر نوع زیرکلاس خودش override می‌کند)
    /// </summary>
    public abstract void HandleMovement();
}