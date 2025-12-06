using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("UI References")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarBackground;
    
    [Header("Visual Feedback")]
    [SerializeField] private Transform characterParent;
    [SerializeField] private float damageFadeDuration = 0.15f;
    [SerializeField] private float damageMinAlpha = 0.3f;
    [SerializeField] private int damageFadeCount = 2;
    
    [Header("Health Bar Animation")]
    [SerializeField] private float healthBarAnimationSpeed = 0.3f;
    [SerializeField] private Ease healthBarEase = Ease.OutQuad;
    
    [Header("Low Health Settings")]
    [SerializeField] private float lowHealthThreshold = 30f;  // درصد سلامتی کم
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color normalHealthColor = Color.green;
    [SerializeField] private bool pulseOnLowHealth = true;
    
    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 2f;
    
    private SpriteRenderer[] allSpriteRenderers;
    private bool isDead = false;
    private Coroutine lowHealthPulseCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        
        if (characterParent != null)
        {
            allSpriteRenderers = characterParent.GetComponentsInChildren<SpriteRenderer>();
        }
        
        UpdateHealthBar();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current Health: {currentHealth}/{maxHealth}");
        
        UpdateHealthBar();
        PlayDamageFeedback();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"{gameObject.name} healed {healAmount}. Current Health: {currentHealth}/{maxHealth}");
        
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float targetFill = currentHealth / maxHealth;

        // Kill previous tweens to avoid stacking
        healthBarFill.DOKill();
        healthBarFill.transform.DOKill();

        // ===== 1) Smooth Fill Amount =====
        healthBarFill
            .DOFillAmount(targetFill, healthBarAnimationSpeed)
            .SetEase(healthBarEase);

        // ===== 2) Smooth Color Change =====
        Color targetColor = (currentHealth <= lowHealthThreshold) 
            ? lowHealthColor 
            : normalHealthColor;

        healthBarFill
            .DOColor(targetColor, healthBarAnimationSpeed)
            .SetEase(Ease.Linear);

        // ===== 3) Tiny Shake on damage (only when losing health) =====
        if (currentHealth < maxHealth) 
        {
            healthBarFill.transform
                .DOShakePosition(0.2f, 4f, 10, 90f, false, true)
                .SetEase(Ease.OutQuad);
        }
    }




    private void UpdateHealthBarColor()
    {
        if (healthBarFill == null) return;
        
        float healthPercentage = GetHealthPercentage();
        
        Color targetColor = healthPercentage <= lowHealthThreshold ? lowHealthColor : normalHealthColor;
        
        healthBarFill.DOKill();
        healthBarFill.DOColor(targetColor, healthBarAnimationSpeed);
    }

    private IEnumerator PulseLowHealth()
    {
        while (true)
        {
            if (healthBarFill != null)
            {
                // بزرگ شدن
                healthBarFill.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(0.5f);
                
                // کوچک شدن
                healthBarFill.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield break;
            }
        }
    }

    private void PlayDamageFeedback()
    {
        if (allSpriteRenderers != null && allSpriteRenderers.Length > 0)
        {
            StartCoroutine(FlashOnDamage());
        }
    }

    private IEnumerator FlashOnDamage()
    {
        // توقف انیمیشن‌های قبلی
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr != null) sr.DOKill();
        }

        for (int i = 0; i < damageFadeCount; i++)
        {
            // Fade out
            FadeAllSprites(damageMinAlpha, damageFadeDuration);
            yield return new WaitForSeconds(damageFadeDuration);

            // Fade in
            FadeAllSprites(1f, damageFadeDuration);
            yield return new WaitForSeconds(damageFadeDuration);
        }

        SetAllSpritesAlpha(1f);
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

    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log($"{gameObject.name} has died!");
        
        // توقف pulse
        if (lowHealthPulseCoroutine != null)
        {
            StopCoroutine(lowHealthPulseCoroutine);
        }
        
        // اینجا می‌تونی انیمیشن مرگ، صدا و... اضافه کنی
        OnDeath();
        
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    // متدی که می‌تونی override کنی برای رویدادهای مرگ
    protected virtual void OnDeath()
    {
        // مثال: غیرفعال کردن کنترلر، پخش انیمیشن مرگ و...
        // Animator animator = GetComponent<Animator>();
        // if (animator != null) animator.SetTrigger("Death");
    }

    // متدهای کمکی عمومی
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => (currentHealth / maxHealth) * 100f;
    public bool IsDead() => isDead;
    public bool IsFullHealth() => currentHealth >= maxHealth;
    private void OnDisable()
    {
        // پاکسازی همه Tween ها
        if (allSpriteRenderers != null)
        {
            foreach (SpriteRenderer sr in allSpriteRenderers)
            {
                if (sr != null) sr.DOKill();
            }
        }
        
        if (healthBarFill != null)
        {
            healthBarFill.DOKill();
        }
    }
}
