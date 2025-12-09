using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Components")]
    public EnemyMobility mobility;
    public EnemyAttack attack;
    public Transform player;
    
    [Header("Stats")]
    public float health = 100f;

    [Header("Camera Activation")]
    public bool useCameraActivation = true;
    public float activationDistance = 5f;

    private Camera mainCamera;
    private bool isActive = false;
    private Animator animator;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();

        mobility?.Initialize(this);
        attack?.Initialize(this);

        if (useCameraActivation)
            SetActive(false);
    }

    protected virtual void Update()
    {
        if (useCameraActivation)
            CheckCameraActivation();

        if (!useCameraActivation || isActive)
        {
            mobility?.HandleMovement();
            attack?.HandleAttack();
        }
    }

    void CheckCameraActivation()
    {
        if (mainCamera == null) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        float buffer = activationDistance / (mainCamera.orthographicSize * 2f);
        
        bool inView = viewPos.z > 0 && 
                      viewPos.x > -buffer && viewPos.x < 1 + buffer &&
                      viewPos.y > -buffer && viewPos.y < 1 + buffer;

        if (inView != isActive)
            SetActive(inView);
    }

    void SetActive(bool active)
    {
        isActive = active;

        if (mobility != null) mobility.enabled = active;
        if (attack != null) attack.enabled = active;
        
        if (animator != null)
        {
            animator.enabled = active;
            animator.SetBool("Start", active);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!useCameraActivation) return;

        Gizmos.color = isActive ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
