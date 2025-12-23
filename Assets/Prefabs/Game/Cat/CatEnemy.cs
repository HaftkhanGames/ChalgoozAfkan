using UnityEngine;
using System.Collections;

public class CatEnemy : MonoBehaviour
{
    // =========================
    // ENUMS
    // =========================

    public enum MoveDirection { Left, Right, Up, Down }
    private enum CatState { Moving, PreparingLeap, Leaping }

    // =========================
    // STATE
    // =========================

    private CatState currentState = CatState.Moving;

    // =========================
    // MOVEMENT
    // =========================

    [Header("🚶 Movement")]
    public MoveDirection direction = MoveDirection.Right;
    public float walkSpeed = 2.5f;
    public float runSpeed = 4f;
    public bool flipSprite = true;
    public Transform spriteTransform;

    private Vector3 moveDir;
    private bool run;

    // =========================
    // TARGET
    // =========================

    [Header("🎯 Player")]
    public Transform player;
    public float stopRange = 6f;

    // =========================
    // LEAP ATTACK
    // =========================

    [Header("🦘 Leap Attack")]
    public float preJumpDelay = 0.25f;
    public float jumpDuration = 0.7f;
    public float jumpHeight = 2f;
    public float leapCooldown = 1.5f;
    
    // زمان فعال بودن Collider حمله هنگام پرش
    [Tooltip("مدت زمان فعال ماندن Collider حمله بعد از شروع پرش (مثلاً 0.15s)")]
    public float attackActiveTime = 0.15f; 

    public AnimationCurve jumpCurve =
        new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.5f, 1),
            new Keyframe(1, 0)
        );

    private float nextLeapTime;

    // =========================
    // DAMAGE (مثل MeleeAttack)
    // =========================

    [Header("💥 Damage")]
    [Tooltip("Collider حمله که باید Is Trigger باشد")]
    public Collider2D attackCollider; 
    public int attackDamage = 1;

    // متغیرهای داخلی برای Damage
    private PlayerHealth cachedPlayerHealth;
    private bool hasHitPlayer; 

    // =========================
    // ANIMATOR
    // =========================

    [Header("🎬 Animator")]
    public Animator animator;
    public string movingBool = "Moving";
    public string runBool = "Run";
    public string leapTrigger = "Leap";

    // =========================
    // COMPONENTS
    // =========================

    private Rigidbody2D rb;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (spriteTransform == null)
            spriteTransform = transform;

        moveDir = GetDirectionVector(direction).normalized;
        UpdateFacing();
    }

    private void Update()
    {
        if (player == null)
            return;

        switch (currentState)
        {
            case CatState.Moving:
                HandleMovement();
                TryLeapAttack();
                break;

            case CatState.PreparingLeap:
            case CatState.Leaping:
                // کنترل کامل داخل Coroutine
                break;
        }
    }

    // =========================
    // MOVEMENT LOGIC
    // =========================

    private void HandleMovement()
    {
        float speed = run ? runSpeed : walkSpeed;

        transform.position += moveDir * speed * Time.deltaTime;

        animator?.SetBool(movingBool, true);
        animator?.SetBool(runBool, run);
    }

    // =========================
    // LEAP CHECK
    // =========================

    private void TryLeapAttack()
    {
        if (Time.time < nextLeapTime)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= stopRange)
        {
            nextLeapTime = Time.time + leapCooldown;
            StartCoroutine(LeapAttackRoutine());
        }
    }

    // =========================
    // LEAP ROUTINE
    // =========================

    private IEnumerator LeapAttackRoutine()
    {
        if (currentState != CatState.Moving)
            yield break;

        currentState = CatState.PreparingLeap;
        run = false;
        hasHitPlayer = false; // ریست کردن وضعیت دمیج

        // 🔥 انیمیشن بلافاصله اجرا می‌شود
        animator?.SetBool(movingBool, false);
        animator?.SetBool(runBool, false);
        animator?.SetTrigger(leapTrigger);

        // فریز فیزیک (اگرچه در آماده‌سازی حرکت نمی‌کند، اما برای ثبات بهتر است)
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            rb.simulated = false;
        }

        // ⏳ delay مخصوص شروع پرش واقعی
        yield return new WaitForSeconds(preJumpDelay);

        // 🔥 شروع پرش واقعی
        currentState = CatState.Leaping;

        // فعال‌سازی Collider و شروع تایمر غیرفعال‌سازی
        if (attackCollider != null)
        {
            StartCoroutine(DisableAttackColliderAfterDelay());
        }


        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(
            player.position.x,
            startPos.y, // فرود دقیق روی زمین (Y اولیه)
            startPos.z
        );

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += jumpCurve.Evaluate(t) * jumpHeight;

            transform.position = pos;

            yield return null;
        }

        transform.position = targetPos; // تضمین فرود دقیق

        // 🟢 بازگشت به حالت عادی + فعال کردن run
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        run = true;
        animator?.SetBool(runBool, true);

        currentState = CatState.Moving;
    }

    private IEnumerator DisableAttackColliderAfterDelay()
    {
        yield return new WaitForSeconds(attackActiveTime);
    }

    // =========================
    // DAMAGE LOGIC (مثل MeleeAttack)
    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("1");
        // فقط وقتی Collider حمله فعال است، دمیج اعمال شود
        if (attackCollider == null || !attackCollider.enabled)
            return;

        // فقط با پلیر برخورد کند
        if (!other.CompareTag("Player"))
            return;

        // فقط یک بار در هر پرش دمیج دهد
        if (hasHitPlayer)
            return;

        // کش کردن PlayerHealth
        if (cachedPlayerHealth == null || cachedPlayerHealth.gameObject != other.gameObject)
        {
            // فرض می‌کنیم PlayerHealth روی گیمپلیری است که Collider دارد
            cachedPlayerHealth = other.GetComponent<PlayerHealth>();
        }

        if (cachedPlayerHealth != null)
        {
            // ✅ کم کردن سلامتی
            cachedPlayerHealth.TakeDamage(attackDamage);
            
            // جلوگیری از دمیج چندباره
            hasHitPlayer = true;
            
            // غیرفعال کردن کلایدر برای جلوگیری از دمیج مجدد (مثل MeleeAttack)
            attackCollider.enabled = false;
        }
    }

    // =========================
    // HELPERS
    // =========================

    private Vector3 GetDirectionVector(MoveDirection dir)
    {
        return dir switch
        {
            MoveDirection.Left  => Vector3.left,
            MoveDirection.Right => Vector3.right,
            MoveDirection.Up    => Vector3.up,
            _                   => Vector3.down
        };
    }

    private void UpdateFacing()
    {
        if (!flipSprite || spriteTransform == null)
            return;

        Vector3 scale = spriteTransform.localScale;
        scale.x = (direction == MoveDirection.Right)
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        spriteTransform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
        
        // این دیگر استفاده نمی‌شود، ولی برای مرجع می‌گذاریم
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
