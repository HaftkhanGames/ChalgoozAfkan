using System.Collections;
using UnityEngine;

/// <summary>
/// Projectile attack system for shooter enemies.
/// Completely independent of mobility - only handles shooting logic.
/// Supports both single-shot and continuous attack modes.
/// </summary>
public class ProjectileAttack : EnemyAttack
{
    #region Inspector Fields
    
    [Header("🎯 Projectile Settings")]
    [Tooltip("Projectile prefab to spawn")]
    public GameObject projectilePrefab;
    
    [Tooltip("Point where projectiles spawn (if null, uses enemy position + height offset)")]
    public Transform shootPoint;
    
    [Tooltip("Projectile movement speed")]
    public float projectileSpeed = 15f;
    
    [Tooltip("Height offset if shootPoint is null")]
    public float spawnHeightOffset = 1f;
    
    [Header("⏱️ Attack Timing")]
    
    [Tooltip("Delay before shooting after animation starts")]
    public float shootDelay = 0.5f;
    
    [Header("🎬 Animation (Optional)")]
    [Tooltip("Animator component (leave null if no animation needed)")]
    public Animator animator;
    
    [Tooltip("Attack animation state name")]
    public string attackStateName = "Attack";
    
    [Tooltip("Should play attack animation?")]
    public bool useAnimation = true;
    
    [Header("🔫 Attack Mode")]
    [Tooltip("If true: attacks once then stops. If false: attacks continuously")]
    public bool singleShotMode = false;
    
    [Header("📏 Attack Range")]
    [Tooltip("Maximum distance to start attacking the player")]
    public float attackRange = 6f;

    [Tooltip("Automatically enable attack when player enters range")]
    public bool autoEnableByRange = true;

    [Tooltip("Stop attacking when player leaves range")]
    public bool disableWhenOutOfRange = true;

    #endregion

    #region Private Fields
    
    /// <summary>
    /// Whether this attack component is allowed to fire
    /// </summary>
    private bool isAttackEnabled = false;
    
    /// <summary>
    /// Prevents multiple simultaneous attack sequences
    /// </summary>
    private bool isExecutingAttack = false;
    
    /// <summary>
    /// Tracks if single-shot has been fired (only for singleShotMode)
    /// </summary>
    private bool hasFiredSingleShot = false;
    
    [Header("🪞 Facing / Flip Settings")]
    [SerializeField] private Transform spriteTransform; 
    [SerializeField] private bool facePlayer = true;

    private bool isFacingRight = true;

    #endregion

    #region Initialization
    
    public override void Initialize(EnemyBase baseEnemy)
    {
        base.Initialize(baseEnemy);

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteTransform == null)
            spriteTransform = transform; // fallback

        isFacingRight = spriteTransform.localScale.x > 0;

        nextAttackTime = 0f;
        isAttackEnabled = false;
        hasFiredSingleShot = false;
    }

    
    #endregion

    #region Unity Callbacks
    
    private void OnEnable()
    {
        // Reset state when component is enabled
        isAttackEnabled = false;
        isExecutingAttack = false;
        hasFiredSingleShot = false;
        nextAttackTime = 0f;
    }
    
    #endregion

    #region Attack Logic
    
    /// <summary>
    /// Main attack handler called by EnemyBase every frame
    /// </summary>
    public override void HandleAttack()
    {
        HandleFacing();
        HandleRangeCheck();

        if (!CanExecuteAttack()) return;

        StartCoroutine(AttackSequence());

        nextAttackTime = Time.time + attackCooldown;

        if (singleShotMode)
            hasFiredSingleShot = true;
    }


    /// <summary>
    /// Automatically enables/disables attack based on player distance
    /// </summary>
    private void HandleRangeCheck()
    {
        if (!autoEnableByRange) return;
        if (!HasValidPlayer()) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (!isAttackEnabled)
                EnableAttack();
        }
        else
        {
            if (disableWhenOutOfRange && isAttackEnabled)
                DisableAttack();
        }
    }

    
    /// <summary>
    /// Check if all conditions are met to execute an attack
    /// </summary>
    private bool CanExecuteAttack()
    {
        // Not enabled yet
        if (!isAttackEnabled) return false;
        
        // Already executing an attack
        if (isExecutingAttack) return false;
        
        // Cooldown not ready
        if (Time.time < nextAttackTime) return false;
        
        // Single-shot already fired
        if (singleShotMode && hasFiredSingleShot) return false;
        
        // Missing required references
        if (projectilePrefab == null) return false;
        if (!HasValidPlayer()) return false;
        
        return true;
    }
    
    /// <summary>
    /// Attack sequence with animation and shoot delay
    /// </summary>
    private IEnumerator AttackSequence()
    {
        isExecutingAttack = true;
        
        // Play attack animation if enabled
        if (useAnimation && animator != null)
        {
            animator.Play(attackStateName, -1, 0f);
        }
        
        // Wait for shoot delay
        yield return new WaitForSeconds(shootDelay);
        
        // Execute actual shooting
        ShootProjectile();
        
        isExecutingAttack = false;
    }
    
    /// <summary>
    /// Spawn and configure projectile
    /// </summary>
    private void ShootProjectile()
    {
        // Determine spawn position
        Vector3 spawnPosition = shootPoint != null 
            ? shootPoint.position 
            : transform.position + Vector3.up * spawnHeightOffset;
        
        // Calculate direction to player
        Vector3 direction = (player.position - spawnPosition).normalized;
        
        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        
        // Setup projectile (assuming it has RockProjectile or similar component)
        RockProjectile rockProjectile = projectile.GetComponent<RockProjectile>();
        if (rockProjectile != null)
        {
            rockProjectile.Setup(direction, projectileSpeed);
        }
        else
        {
            // Fallback: try to apply velocity if it has Rigidbody2D
            Rigidbody2D rb2d = projectile.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.linearVelocity = direction * projectileSpeed;
            }
        }
    }
    
    #endregion

    #region Public Control Methods
    
    /// <summary>
    /// Flip enemy to face the player if they pass by
    /// </summary>
    private void HandleFacing()
    {
        if (!facePlayer) return;
        if (!HasValidPlayer()) return;

        bool shouldFaceRight = player.position.x > transform.position.x;

        if (shouldFaceRight != isFacingRight)
        {
            Flip(shouldFaceRight);
        }
    }

    private void Flip(bool faceRight)
    {
        isFacingRight = faceRight;

        Vector3 scale = spriteTransform.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceRight ? 1 : -1);
        spriteTransform.localScale = scale;
    }

    /// <summary>
    /// Enable attack system - call this when you want enemy to start attacking
    /// </summary>
    public void EnableAttack()
    {
        isAttackEnabled = true;
    }
    
    /// <summary>
    /// Disable attack system - call this when you want enemy to stop attacking
    /// </summary>
    public void DisableAttack()
    {
        isAttackEnabled = false;
        
        // Stop any ongoing attack
        StopAllCoroutines();
        isExecutingAttack = false;
    }
    
    /// <summary>
    /// Reset single-shot mode (allows firing again)
    /// </summary>
    public void ResetSingleShot()
    {
        hasFiredSingleShot = false;
    }
    
    /// <summary>
    /// Check if attack is currently enabled
    /// </summary>
    public bool IsAttackEnabled()
    {
        return isAttackEnabled;
    }
    
    #endregion

    #region Gizmos
    
    private void OnDrawGizmosSelected()
    {
        // Draw shoot point
        Vector3 shootPosition = shootPoint != null 
            ? shootPoint.position 
            : transform.position + Vector3.up * spawnHeightOffset;
        
        // Color based on attack state
        if (Application.isPlaying)
        {
            Gizmos.color = isAttackEnabled ? Color.green : Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }
        
        Gizmos.DrawWireSphere(shootPosition, 0.3f);
        Gizmos.DrawLine(shootPosition, shootPosition + Vector3.up * 0.5f);
        
        // Draw attack direction indicator
        if (Application.isPlaying && player != null)
        {
            Vector3 direction = (player.position - shootPosition).normalized;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(shootPosition, direction * 2f);
        }
        
        // Draw mode indicator
        if (singleShotMode)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 2f, Vector3.one * 0.3f);
        }
    }
    
    #endregion
}
