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

    [Header("Wing Burst VFX ✅")]
    [SerializeField] private BirdWingBurstVFX wingBurstPrefab;
    [SerializeField] private Transform wingBurstPoint;
    [SerializeField] private bool playWingBurstOnDamage = true;

    [Header("Health Bar Animation")]
    [SerializeField] private float healthBarAnimationSpeed = 0.3f;
    [SerializeField] private Ease healthBarEase = Ease.OutQuad;

    [Header("Low Health Settings")]
    [SerializeField] private float lowHealthThreshold = 30f;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color normalHealthColor = Color.green;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 2f;

    private SpriteRenderer[] allSpriteRenderers;
    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;

        if (characterParent != null)
            allSpriteRenderers = characterParent.GetComponentsInChildren<SpriteRenderer>();

        if (wingBurstPoint == null)
            wingBurstPoint = transform;

        UpdateHealthBar();
    }

    // ===================== DAMAGE =====================
    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        Debug.Log($"Player took {damageAmount} damage → {currentHealth}/{maxHealth}");

        UpdateHealthBar();
        PlayDamageFeedback();
        PlayWingBurst();

        if (currentHealth <= 0)
            Die();
    }

    // ===================== HEAL =====================
    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        UpdateHealthBar();
    }

    // ===================== WING VFX =====================
    private void PlayWingBurst()
    {
        if (!playWingBurstOnDamage) return;
        if (wingBurstPrefab == null) return;

        BirdWingBurstVFX vfx = Instantiate(
            wingBurstPrefab,
            wingBurstPoint.position,
            Quaternion.identity
        );

        vfx.Play(wingBurstPoint.position);
    }

    // ===================== HEALTH BAR =====================
    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float targetFill = currentHealth / maxHealth;

        healthBarFill.DOKill();
        healthBarFill.transform.DOKill();

        healthBarFill
            .DOFillAmount(targetFill, healthBarAnimationSpeed)
            .SetEase(healthBarEase);

        Color targetColor =
            currentHealth <= lowHealthThreshold
                ? lowHealthColor
                : normalHealthColor;

        healthBarFill
            .DOColor(targetColor, healthBarAnimationSpeed)
            .SetEase(Ease.Linear);

        if (currentHealth < maxHealth)
        {
            healthBarFill.transform
                .DOShakePosition(0.2f, 4f, 10, 90f, false, true);
        }
    }

    // ===================== DAMAGE FLASH =====================
    private void PlayDamageFeedback()
    {
        if (allSpriteRenderers != null && allSpriteRenderers.Length > 0)
            StartCoroutine(FlashOnDamage());
    }

    private IEnumerator FlashOnDamage()
    {
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr != null) sr.DOKill();
        }

        for (int i = 0; i < damageFadeCount; i++)
        {
            FadeAllSprites(damageMinAlpha, damageFadeDuration);
            yield return new WaitForSeconds(damageFadeDuration);

            FadeAllSprites(1f, damageFadeDuration);
            yield return new WaitForSeconds(damageFadeDuration);
        }
    }

    private void FadeAllSprites(float alpha, float duration)
    {
        foreach (SpriteRenderer sr in allSpriteRenderers)
        {
            if (sr != null)
                sr.DOFade(alpha, duration).SetEase(Ease.InOutSine);
        }
    }

    // ===================== DEATH =====================
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died");

        OnDeath();

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

    protected virtual void OnDeath()
    {
        // اینجا انیمیشن مرگ یا GameOver
    }

    private void OnDisable()
    {
        if (healthBarFill != null)
            healthBarFill.DOKill();

        if (allSpriteRenderers != null)
        {
            foreach (SpriteRenderer sr in allSpriteRenderers)
            {
                if (sr != null) sr.DOKill();
            }
        }
    }
}
