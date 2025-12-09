using System.Collections;
using UnityEngine;

public class ProjectileAttack : EnemyAttack
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 15f;
    public float spawnHeight = 1f;
    
    [Header("Timing")]
    public float cooldown = 2f;
    public float shootDelay = 0.5f;
    public float requiredWalkDistance = 3f;
    
    [Header("Animation")]
    public Animator animator;
    public string attackStateName = "Attack";
    
    [Header("Player Pass Detection")]
    public bool rotateWhenPlayerPasses = true;
    
    private bool canAttack = false;
    private bool isAttacking = false;
    private float nextShootTime = 0f;
    private Vector3 startPosition;
    private bool playerHasPassed = false;
    private bool hasRotated = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        canAttack = false;
        playerHasPassed = false;
        hasRotated = false;
        startPosition = transform.position;
    }

    void Update()
    {
        // فعال کردن حمله بعد از طی کردن مسافت
        if (!canAttack && Vector3.Distance(startPosition, transform.position) >= requiredWalkDistance)
        {
            canAttack = true;
        }

        // چک کردن پاس شدن پلیر
        if (rotateWhenPlayerPasses && !hasRotated && player != null)
        {
            CheckPlayerPass();
        }
    }

    void CheckPlayerPass()
    {
        float enemyX = transform.position.x;
        float playerX = player.position.x;

        // اگه پلیر 2 واحد جلوتر رفت (بسته به جهت حرکت اصلی)
        // اینجا فرض میکنیم بازی به سمت راست میره
        if (playerX > enemyX + 2f && !playerHasPassed)
        {
            playerHasPassed = true;
            RotateToFacePlayer();
        }
    }

    void RotateToFacePlayer()
    {
        // چرخش 180 درجه روی محور Y
        transform.Rotate(0f, 180f, 0f);
        hasRotated = true;
    }

    public override void HandleAttack()
    {
        if (!canAttack || isAttacking || Time.time < nextShootTime) return;
        if (player == null || projectilePrefab == null) return;

        StartCoroutine(AttackSequence());
        nextShootTime = Time.time + cooldown;
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        
        animator?.Play(attackStateName, -1, 0f);
        yield return new WaitForSeconds(shootDelay);
        
        Shoot();
        isAttacking = false;
    }

    void Shoot()
    {
        Vector3 startPos = shootPoint != null 
            ? shootPoint.position 
            : transform.position + Vector3.up * spawnHeight;
        
        Vector3 direction = (player.position - startPos).normalized;
        
        GameObject bullet = Instantiate(projectilePrefab, startPos, Quaternion.identity);
        bullet.GetComponent<RockProjectile>()?.Setup(direction, projectileSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 shootPos = shootPoint != null 
            ? shootPoint.position 
            : transform.position + Vector3.up * spawnHeight;
        
        Gizmos.color = canAttack ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(shootPos, 0.3f);
        
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, requiredWalkDistance);
        }

        // نشون دادن وضعیت چرخش
        if (Application.isPlaying && hasRotated)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }
}
