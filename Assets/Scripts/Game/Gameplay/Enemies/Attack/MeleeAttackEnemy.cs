using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MeleeAttack : EnemyAttack
{
    [Header("Collider Settings")]
    public Collider2D attackCollider;
    public float activeDuration = 0.2f;
    public FaceExpressionSystem faceExpressionSystem;

    [Header("Attack Once Settings")]
    public bool attackOnce = false;
    private bool hasAttacked = false;

    [Header("Hit Flash Settings")]
    public Transform targetCharacterParent;
    public float flashDuration = 2f;
    public float fadeDuration = 0.15f;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;
    public int flashCount = 6;

    private SpriteRenderer[] allSpriteRenderers;
    private PlayerHealth cachedPlayerHealth;

    private void Start()
    {
        if (targetCharacterParent != null)
        {
            allSpriteRenderers = targetCharacterParent.GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public override void HandleAttack()
    {
        if (player == null || attackCollider == null) return;

        if (attackOnce && hasAttacked) return;

        float distance = Vector2.Distance(enemy.transform.position, player.position);
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(ActivateCollider());
        }
    }

    private IEnumerator ActivateCollider()
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(activeDuration);
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackCollider.enabled && other.CompareTag("Player"))
        {
            // دریافت یا کش کردن PlayerHealth
            if (cachedPlayerHealth == null)
            {
                cachedPlayerHealth = other.GetComponent<PlayerHealth>();
            }

            // اعمال آسیب به پلیر
            if (cachedPlayerHealth != null)
            {
                cachedPlayerHealth.TakeDamage(attackDamage);
                Debug.Log($"{enemy.name} hit {other.name} with Melee Attack! Damage: {attackDamage}");
            }
            else
            {
                Debug.LogWarning($"PlayerHealth component not found on {other.name}!");
            }

            // نمایش حالت صورت
            if (faceExpressionSystem != null)
            {
                faceExpressionSystem.ShowExpression(FaceExpressionSystem.ExpressionType.Dead, 0.3f);
            }

            // افکت بصری fade (فقط اگه اسپرایت‌ها ست شده باشن)
            if (allSpriteRenderers != null && allSpriteRenderers.Length > 0)
            {
                StartCoroutine(FlashAllSpritesSmooth());
            }

            // علامت‌گذاری حمله انجام شده (اگه attackOnce فعال باشه)
            if (attackOnce)
            {
                hasAttacked = true;
            }
        }
    }

    private IEnumerator FlashAllSpritesSmooth()
    {
        // توقف همه انیمیشن‌های قبلی
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr != null)
            {
                sr.DOKill();
            }
        }

        // انجام چشمک‌های متوالی
        for (int i = 0; i < flashCount; i++)
        {
            // Fade out
            FadeAllSprites(minAlpha, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);

            // Fade in
            FadeAllSprites(maxAlpha, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);
        }

        // اطمینان از نمایش کامل در پایان
        SetAllSpritesAlpha(maxAlpha);
    }

    private void FadeAllSprites(float targetAlpha, float duration)
    {
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr != null)
            {
                sr.DOFade(targetAlpha, duration).SetEase(Ease.InOutSine);
            }
        }
    }

    private void SetAllSpritesAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }
        }
    }

    public void ResetAttack()
    {
        hasAttacked = false;
    }

    private void OnDisable()
    {
        if (allSpriteRenderers != null)
        {
            foreach (SpriteRenderer sr in allSpriteRenderers)
            {
                if (sr != null)
                {
                    sr.DOKill();
                }
            }
        }
    }

    // متد کمکی برای تغییر دادن damage از خارج
    public void SetDamage(float newDamage)
    {
        attackDamage = newDamage;
    }

    // متد کمکی برای چک کردن آیا حمله انجام شده
    public bool HasAttacked()
    {
        return hasAttacked;
    }
}
