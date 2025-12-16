using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy System/Enemy Stats")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("General")]
    public string enemyName;
    public float maxHealth = 100f;
    public int scoreValue = 10; // امتیاز کشتن این دشمن

    [Header("Combat")]
    public float damage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 3f;
}