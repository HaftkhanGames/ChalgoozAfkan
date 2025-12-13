using UnityEngine;

/// <summary>
/// Abstract base class for all enemy attack behaviors.
/// Handles common attack functionality like cooldowns and damage calculation.
/// Derived classes implement specific attack patterns (projectile, melee, ranged, etc.)
/// </summary>
public abstract class EnemyAttack : MonoBehaviour
{
    #region Inspector Fields
    
    [Header("Attack Settings")]
    [Tooltip("Base damage dealt by this attack")]
    public float attackDamage = 20f;
    
    [Tooltip("Time between attacks in seconds")]
    public float attackCooldown = 1.5f;
    
    #endregion

    #region Protected Fields
    
    /// <summary>
    /// Reference to the parent EnemyBase component
    /// </summary>
    protected EnemyBase enemy;
    
    /// <summary>
    /// Cached reference to player transform (from EnemyBase)
    /// </summary>
    protected Transform player;
    
    /// <summary>
    /// Time when next attack can be executed
    /// </summary>
    protected float nextAttackTime;
    
    #endregion

    #region Initialization
    
    /// <summary>
    /// Initialize attack component with reference to parent enemy.
    /// Called by EnemyBase during Start()
    /// </summary>
    /// <param name="baseEnemy">Parent EnemyBase component</param>
    public virtual void Initialize(EnemyBase baseEnemy)
    {
        enemy = baseEnemy;
        
        // Use the optimized Player property from EnemyBase (cached reference)
        player = baseEnemy.Player;
        
        // Initialize cooldown timer
        nextAttackTime = Time.time;
    }
    
    #endregion

    #region Abstract Methods
    
    /// <summary>
    /// Execute attack behavior - must be implemented by derived classes.
    /// Called every frame by EnemyBase.Update() when enemy is active.
    /// Examples: ProjectileAttack, MeleeAttack, RangedAttack, etc.
    /// </summary>
    public abstract void HandleAttack();
    
    #endregion

    #region Helper Methods
    
    /// <summary>
    /// Check if attack is ready based on cooldown timer
    /// </summary>
    /// <returns>True if enough time has passed since last attack</returns>
    protected bool IsAttackReady()
    {
        return Time.time >= nextAttackTime;
    }
    
    /// <summary>
    /// Reset attack cooldown timer after executing an attack
    /// </summary>
    protected void ResetAttackCooldown()
    {
        nextAttackTime = Time.time + attackCooldown;
    }
    
    /// <summary>
    /// Check if player reference is valid
    /// </summary>
    /// <returns>True if player exists and is not destroyed</returns>
    protected bool HasValidPlayer()
    {
        return player != null;
    }
    
    /// <summary>
    /// Calculate distance to player (squared for performance)
    /// Use this instead of Vector3.Distance to avoid square root calculation
    /// </summary>
    /// <returns>Squared distance to player</returns>
    protected float GetSquaredDistanceToPlayer()
    {
        if (!HasValidPlayer()) return float.MaxValue;
        return (player.position - transform.position).sqrMagnitude;
    }
    
    /// <summary>
    /// Calculate actual distance to player
    /// Use GetSquaredDistanceToPlayer() when possible for better performance
    /// </summary>
    /// <returns>Distance to player in world units</returns>
    protected float GetDistanceToPlayer()
    {
        if (!HasValidPlayer()) return float.MaxValue;
        return Vector3.Distance(transform.position, player.position);
    }
    
    #endregion
}
