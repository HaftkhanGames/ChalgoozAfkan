using UnityEngine;

/// <summary>
/// Optimized base class for all enemy types in the game.
/// Manages common functionality including health, camera-based activation,
/// and component coordination for 20+ enemy variants.
/// </summary>
public class EnemyBase : MonoBehaviour
{
    #region Inspector Fields
    
    [Header("Components")]
    [Tooltip("Reference to the enemy's mobility component")]
    public EnemyMobility mobility;
    
    [Tooltip("Reference to the enemy's attack component")]
    public EnemyAttack attack;
    
    [Header("Stats")]
    [Tooltip("Current health of the enemy")]
    public float health = 100f;

    [Header("Camera Activation")]
    [Tooltip("Enable/disable camera-based activation system")]
    public bool useCameraActivation = true;
    
    [Tooltip("Buffer distance beyond viewport for activation (in world units)")]
    public float activationBuffer = 2f;
    
    #endregion

    #region Private Fields
    
    // Cached references (optimization: avoid repeated lookups)
    private Transform playerTransform;
    private Camera mainCamera;
    private Animator animator;
    
    // State management
    private bool isActive = false;
    private bool isInitialized = false;
    
    // Camera optimization: cache calculated buffer to avoid per-frame recalculation
    private float cachedBuffer;
    private float lastCameraSize;
    
    #endregion

    #region Public Properties
    
    /// <summary>
    /// Public accessor for player transform (cached reference)
    /// </summary>
    public Transform Player => playerTransform;
    
    /// <summary>
    /// Returns whether this enemy is currently active
    /// </summary>
    public bool IsActive => isActive;
    
    #endregion

    #region Unity Lifecycle
    
    /// <summary>
    /// Cache all references before Start() to ensure proper initialization order
    /// </summary>
    protected virtual void Awake()
    {
        // Cache camera reference once
        mainCamera = Camera.main;
        
        // Cache animator component
        animator = GetComponent<Animator>();
        
        // Cache player reference once (avoid FindGameObjectWithTag every frame)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    /// <summary>
    /// Initialize enemy components and set initial state
    /// </summary>
    protected virtual void Start()
    {
        // Initialize mobility and attack components
        mobility?.Initialize(this);
        attack?.Initialize(this);
        
        isInitialized = true;

        // Setup activation system
        if (useCameraActivation)
        {
            UpdateCameraBuffer();
            SetActive(false); // Start deactivated, will activate when in view
        }
        else
        {
            SetActive(true); // Always active if camera activation is disabled
        }
    }

    /// <summary>
    /// Main update loop - handles activation check and component updates
    /// </summary>
    protected virtual void Update()
    {
        if (!isInitialized) return;

        // Check camera activation state
        if (useCameraActivation)
        {
            CheckCameraActivation();
        }

        // Execute component logic only when active (optimization for 20+ enemies)
        if (isActive)
        {
            mobility?.HandleMovement();
            attack?.HandleAttack();
        }
    }

    /// <summary>
    /// Cleanup when object is disabled
    /// </summary>
    void OnDisable()
    {
        if (isActive)
        {
            SetActive(false);
        }
    }
    
    #endregion

    #region Camera Activation System
    
    /// <summary>
    /// Check if enemy is within camera viewport and update activation state
    /// Optimized to reduce overhead for multiple enemies
    /// </summary>
    void CheckCameraActivation()
    {
        if (mainCamera == null) return;

        // Check if camera size changed (for dynamic camera adjustments)
        if (Mathf.Abs(mainCamera.orthographicSize - lastCameraSize) > 0.01f)
        {
            UpdateCameraBuffer();
        }

        // Convert world position to viewport space (0-1 range)
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        
        // Check if within viewport bounds plus buffer
        // viewPos.z > 0 ensures object is in front of camera
        bool inView = viewPos.z > 0 && 
                      viewPos.x > -cachedBuffer && 
                      viewPos.x < 1f + cachedBuffer &&
                      viewPos.y > -cachedBuffer && 
                      viewPos.y < 1f + cachedBuffer;

        // Only change state if activation status has changed (avoid redundant calls)
        if (inView != isActive)
        {
            SetActive(inView);
        }
    }

    /// <summary>
    /// Update cached buffer calculation based on current camera size
    /// Only called when camera size changes
    /// </summary>
    void UpdateCameraBuffer()
    {
        if (mainCamera == null) return;
        
        lastCameraSize = mainCamera.orthographicSize;
        // Convert world space buffer to viewport space
        cachedBuffer = activationBuffer / (lastCameraSize * 2f);
    }

    /// <summary>
    /// Enable or disable enemy components based on activation state
    /// </summary>
    /// <param name="active">Target activation state</param>
    void SetActive(bool active)
    {
        isActive = active;

        // Toggle mobility component
        if (mobility != null) 
            mobility.enabled = active;
        
        // Toggle attack component
        if (attack != null) 
            attack.enabled = active;
        
        // Manage animator state
        if (animator != null)
        {
            animator.enabled = active;
            
            // Set animation state only when activating
            if (active)
            {
                animator.SetBool("Start", true);
            }
        }
    }
    
    #endregion

    #region Combat System
    
    /// <summary>
    /// Apply damage to this enemy
    /// Override in derived classes for custom damage behavior
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handle enemy death
    /// Override in derived classes for custom death behavior (animations, effects, loot, etc.)
    /// </summary>
    protected virtual void Die()
    {
        // Default behavior: destroy game object
        // Consider using object pooling instead for better performance
        Destroy(gameObject);
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Manually force deactivation of this enemy
    /// Useful for cleanup or special game events
    /// </summary>
    public void ForceDeactivate()
    {
        if (isActive)
        {
            SetActive(false);
        }
    }
    
    #endregion

    #region Editor Visualization
    
#if UNITY_EDITOR
    /// <summary>
    /// Draw gizmos in editor for debugging activation system
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!useCameraActivation) return;

        // Draw sphere to indicate activation state
        Gizmos.color = isActive ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw activation bounds in viewport
        if (mainCamera != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            float size = mainCamera.orthographicSize;
            float bufferSize = activationBuffer;
            
            // Calculate viewport bounds with buffer
            Gizmos.DrawWireCube(
                mainCamera.transform.position, 
                new Vector3(
                    (size * 2f * mainCamera.aspect) + bufferSize * 2f, 
                    size * 2f + bufferSize * 2f, 
                    1f
                )
            );
        }
    }
#endif
    
    #endregion
}
