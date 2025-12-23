using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MeleeAttack : EnemyAttack
{
    public enum CollisionReaction { None, Shake, Fall }

    [Header("Collider Settings")]
    public Collider2D attackCollider;
    public float activeDuration = 0.2f;

    [Header("Visual Feedback (Self)")]
    [Tooltip("سیستم چهره خود دشمن (اگر کاراکتر زنده است)")]
    public FaceExpressionSystem faceExpressionSystem;

    [Header("Environment Reaction")]
    [Tooltip("نوع واکنش آبجکت هنگام برخورد با پلیر")]
    public CollisionReaction reactionType = CollisionReaction.None;
    
    [Tooltip("آبجکتی که باید بچرخد (اگر خالی باشد، همین آبجکت استفاده می‌شود)")]
    public Transform reactionTarget;
    
    [Tooltip("قدرت لرزش (درجه) یا سرعت افتادن")]
    public float reactionStrength = 10f; 
    
    public UnityEvent OnHitReaction; 

    [Header("Attack Once Settings")]
    [Tooltip("آیا این حمله فقط یک بار انجام می‌شود؟ (مناسب برای تله‌ها و موانع)")]
    public bool attackOnce = false;
    private bool hasAttacked = false;
    
    // متغیرهای کش شده
    private PlayerHealth cachedPlayerHealth;
    private Quaternion initialRotation;
    private Vector3 initialPosition; // برای ریست کردن دقیق پوزیشن

    public override void Initialize(EnemyBase baseEnemy)
    {
        base.Initialize(baseEnemy);
        
        if (reactionTarget == null) reactionTarget = transform;
        
        // ذخیره وضعیت اولیه برای ریست کردن احتمالی
        initialRotation = reactionTarget.rotation;
        initialPosition = reactionTarget.position;
    }

    public override void HandleAttack()
    {
        if (player == null || attackCollider == null) return;
        if (attackOnce && hasAttacked) return;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(PerformAttackRoutine());
        }
    }

    private IEnumerator PerformAttackRoutine()
    {
        if (faceExpressionSystem != null)
        {
            faceExpressionSystem.ShowAttack(0.5f); 
        }

        attackCollider.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackCollider.enabled) return;

        if (other.CompareTag("Player"))
        {
            if (attackOnce && hasAttacked) return;

            if (cachedPlayerHealth == null || cachedPlayerHealth.gameObject != other.gameObject)
            {
                cachedPlayerHealth = other.GetComponent<PlayerHealth>();
            }

            if (cachedPlayerHealth != null)
            {
                cachedPlayerHealth.TakeDamage(attackDamage);
                
                if (attackOnce)
                {
                    hasAttacked = true;
                    // غیرفعال کردن کلایدر برای جلوگیری از دمیج مجدد هنگام افتادن روی پلیر
                    attackCollider.enabled = false; 
                }

                HandleObjectReaction();
            }
        }
    }

    private void HandleObjectReaction()
    {
        OnHitReaction?.Invoke(); 

        if (reactionTarget == null) return;

        StopAllCoroutines(); 

        switch (reactionType)
        {
            case CollisionReaction.Shake:
                StartCoroutine(ShakeRoutine());
                break;
            case CollisionReaction.Fall:
                StartCoroutine(FallRoutine());
                break;
        }
    }

    // --- روتین لرزش (Shake) ---
    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        float duration = 0.4f;
        
        // پیدا کردن نقطه پایین برای لرزش (که درخت از ریشه نلرزد، بلکه از بالا بلرزد)
        // اما برای Shake ساده، چرخش حول محور معمولی هم قابل قبول است
        // یا می‌توانیم از تکنیک RotateAround برای لرزش هم استفاده کنیم
        
        while (elapsed < duration)
        {
            float z = Mathf.Sin(elapsed * 50f) * reactionStrength; 
            reactionTarget.rotation = initialRotation * Quaternion.Euler(0, 0, z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        reactionTarget.rotation = initialRotation;
    }

    // --- روتین افتادن (Fall) اصلاح شده ---
    private IEnumerator FallRoutine()
    {
        // 1. پیدا کردن نقطه پایین (Pivot Point واقعی)
        // ما از Bounds کلایدر استفاده می‌کنیم تا پایین‌ترین نقطه را پیدا کنیم
        Bounds bounds = attackCollider.bounds;
        Vector3 pivotPoint = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);

        float currentAngle = 0f;
        float targetAngle = -90f; // افتادن به سمت راست (ساعت‌گرد) -> منفی
        // اگر می‌خواهید بسته به جهت برخورد بیفتد، می‌توانید این را داینامیک کنید
        
        float fallSpeed = reactionStrength * 10f; // ضریب سرعت

        while (Mathf.Abs(currentAngle) < Mathf.Abs(targetAngle))
        {
            // محاسبه مقدار چرخش در این فریم
            float step = fallSpeed * Time.deltaTime;
            
            // جلوگیری از رد شدن از زاویه هدف
            if (Mathf.Abs(currentAngle + step) > Mathf.Abs(targetAngle))
            {
                step = Mathf.Abs(targetAngle) - Mathf.Abs(currentAngle);
            }

            // اعمال جهت (منفی برای ساعت‌گرد)
            float stepWithDirection = (targetAngle > 0) ? step : -step;

            // نکته طلایی: چرخش حول یک نقطه خاص در فضا (پایین درخت)
            reactionTarget.RotateAround(pivotPoint, Vector3.forward, stepWithDirection);
            
            currentAngle += step;
            yield return null;
        }
        
        // برای اطمینان از فیکس شدن نهایی (اختیاری)
        // در RotateAround چون پوزیشن عوض می‌شود، ست کردن دستی روتیشن ممکن است پرش ایجاد کند
        // پس همین‌جا رها می‌کنیم.
    }

    public void ResetAttack()
    {
        hasAttacked = false;
        if (reactionTarget != null) 
        {
            // برای ریست کردن باید هم روتیشن و هم پوزیشن برگردد
            // چون RotateAround پوزیشن آبجکت را هم تغییر داده است
            reactionTarget.rotation = initialRotation;
            reactionTarget.position = initialPosition;
            attackCollider.enabled = false; // اطمینان از غیرفعال بودن
        }
    }

    public void SetDamage(float newDamage)
    {
        attackDamage = newDamage;
    }

    public bool HasAttacked() => hasAttacked;
    
    // جهت دیباگ: نمایش نقطه‌ای که دورش می‌چرخد
    private void OnDrawGizmosSelected()
    {
        if (attackCollider != null)
        {
            Gizmos.color = Color.red;
            Bounds bounds = attackCollider.bounds;
            Vector3 pivotPoint = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            Gizmos.DrawWireSphere(pivotPoint, 0.1f);
        }
    }
}
