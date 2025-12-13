using UnityEngine;

/// <summary>
/// Abstract base class for all enemy movement behaviors.
/// Handles common mobility functionality like speed control and position tracking.
/// Derived classes implement specific movement patterns (patrol, chase, directional, etc.)
/// </summary>
public abstract class EnemyMobility : MonoBehaviour
{
    #region Inspector Fields
    
    [Header("Mobility Settings")]
    [Tooltip("Movement speed in units per second")]
    public float moveSpeed = 2f;
    
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
    /// Initial spawn position of the enemy
    /// Used for patrol paths or return-to-spawn behaviors
    /// </summary>
    protected Vector3 startPos;
    
    #endregion

    #region Initialization
    
    /// <summary>
    /// Initialize mobility component with reference to parent enemy.
    /// Called by EnemyBase during Start()
    /// </summary>
    /// <param name="baseEnemy">Parent EnemyBase component</param>
    public virtual void Initialize(EnemyBase baseEnemy)
    {
        enemy = baseEnemy;
        
        // Use the optimized Player property from EnemyBase (cached reference)
        player = baseEnemy.Player;
        
        // Cache starting position for patrol/return behaviors
        startPos = transform.position;
    }
    
    #endregion

    #region Abstract Methods
    
    /// <summary>
    /// Execute movement behavior - must be implemented by derived classes.
    /// Called every frame by EnemyBase.Update() when enemy is active.
    /// Examples: DirectionalMobility, PatrolMobility, ChaseMobility, etc.
    /// </summary>
    public abstract void HandleMovement();
    
    #endregion

    #region Helper Methods
    
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
    
    /// <summary>
    /// Calculate direction vector to player (normalized)
    /// </summary>
    /// <returns>Normalized direction vector or Vector3.zero if player is invalid</returns>
    protected Vector3 GetDirectionToPlayer()
    {
        if (!HasValidPlayer()) return Vector3.zero;
        return (player.position - transform.position).normalized;
    }
    
    /// <summary>
    /// Check if enemy has returned to starting position (within threshold)
    /// </summary>
    /// <param name="threshold">Distance threshold for considering "at start"</param>
    /// <returns>True if within threshold distance of start position</returns>
    protected bool IsAtStartPosition(float threshold = 0.1f)
    {
        return Vector3.Distance(transform.position, startPos) < threshold;
    }
    
    /// <summary>
    /// Move transform towards target position using moveSpeed
    /// </summary>
    /// <param name="targetPosition">Target world position</param>
    protected void MoveTowards(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            moveSpeed * Time.deltaTime
        );
    }
    
    #endregion
}
