using UnityEngine;
using System.Collections;

public class LeapAttack : EnemyAttack
{
    [Header("Leap Settings")]
    [Tooltip("مدت زمان پرش")]
    public float jumpDuration = 0.8f;

    [Tooltip("ارتفاع پرش")]
    public float jumpHeight = 2f;

    [Tooltip("منحنی پرش")]
    public AnimationCurve jumpCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
        );

    [Header("Timing")]
    [Tooltip("تاخیر قبل از شروع پرش (خیز برداشتن)")]
    public float preJumpDelay = 0.3f;

    [Header("Hit Detection")]
    public float hitRadius = 0.5f;
    public LayerMask playerLayer;

    // References
    private DirectionalMobility mobility;
    private Rigidbody2D rb;
    private Animator animator;

    // State
    private bool isLeaping = false;

    #region Initialization

    public override void Initialize(EnemyBase baseEnemy)
    {
        base.Initialize(baseEnemy);

        mobility = GetComponent<DirectionalMobility>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    #endregion

    #region Attack Entry Point

    public override void HandleAttack()
    {
        if (player == null)
            return;

        if (isLeaping)
            return;

        if (!IsAttackReady())
            return;

        StartCoroutine(LeapRoutine());
    }

    #endregion

    #region Leap Logic

    private IEnumerator LeapRoutine()
    {
        isLeaping = true;
        ResetAttackCooldown();

        // 1️⃣ قفل حرکت عادی
        if (mobility != null)
            mobility.enabled = false;

        // 2️⃣ فریز فیزیک
        bool originalKinematic = false;

        if (rb != null)
        {
            originalKinematic = rb.isKinematic;
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
            rb.simulated = false;
        }

        // 3️⃣ پخش انیمیشن
        if (animator != null)
            animator.SetTrigger("Leap");

        // 4️⃣ مکث قبل پرش
        yield return new WaitForSeconds(preJumpDelay);

        // 5️⃣ محاسبه نقاط پرش
        Vector3 startPos = transform.position;

        // ✅ فقط X پلیر، Y ثابت (زمین)
        Vector3 targetPos = new Vector3(
            player.position.x,
            startPos.y,
            startPos.z
        );

        float elapsed = 0f;
        bool hasHitPlayer = false;

        // 6️⃣ حرکت پرشی
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

            float heightOffset = jumpCurve.Evaluate(t) * jumpHeight;
            pos.y += heightOffset;

            transform.position = pos;

            // Hit Check
            if (!hasHitPlayer)
            {
                if (Physics2D.OverlapCircle(
                        transform.position,
                        hitRadius,
                        playerLayer))
                {
                    DamagePlayer();
                    hasHitPlayer = true;
                }
            }

            yield return null;
        }

        // 7️⃣ اصلاح فرود
        if (Vector2.Distance(transform.position, targetPos) > 0.05f)
            transform.position = targetPos;

        // 8️⃣ بازگردانی فیزیک
        if (rb != null)
        {
            rb.isKinematic = originalKinematic;
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // 9️⃣ فعال‌سازی مجدد حرکت
        if (mobility != null)
            mobility.enabled = true;

        isLeaping = false;
    }

    #endregion

    #region Damage

    private void DamagePlayer()
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(attackDamage);
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (!isLeaping) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }

    #endregion
}
