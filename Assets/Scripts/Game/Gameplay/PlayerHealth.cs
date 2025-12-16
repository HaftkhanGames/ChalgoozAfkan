using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Logic")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invincibilityDuration = 0.5f; // زمان مصونیت بعد از ضربه
    
    // وضعیت‌ها برای خواندن از بیرون (Read Only)
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    [Header("UI - Main Bar")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Gradient healthColorGradient; // رنگ متغیر بر اساس میزان سلامتی

    [Header("UI - Ghost Bar (Effect)")]
    [Tooltip("نوار دوم زیر نوار اصلی برای افکت تاخیری")]
    [SerializeField] private Image ghostHealthBarFill; 
    [SerializeField] private float ghostBarDelay = 0.5f;
    [SerializeField] private float ghostBarSpeed = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private Transform characterModelRoot; // ریشه مدل برای تکان دادن و فلش زدن
    [SerializeField] private int flashCount = 4;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color damageFlashColor = new Color(1, 0.3f, 0.3f, 1); // قرمز روشن

    [Header("VFX References")]
    [SerializeField] private BirdWingBurstVFX wingBurstPrefab;
    [SerializeField] private Transform wingBurstPoint;

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged; // رویداد تغییر سلامتی (برای UI جداگانه یا صدا)
    public UnityEvent OnTakeDamage;           // رویداد ضربه خوردن
    public UnityEvent OnDeath;                // رویداد مرگ

    // متغیرهای داخلی
    private float lastDamageTime;
    private SpriteRenderer[] childRenderers;
    private Color[] originalColors; // ذخیره رنگ‌های اصلی برای بازگرداندن بعد از فلش
    private Tween ghostTween;

    private void Start()
    {
        CurrentHealth = maxHealth;
        IsDead = false;

        // کش کردن رندررها و رنگ‌های اولیه
        if (characterModelRoot != null)
        {
            childRenderers = characterModelRoot.GetComponentsInChildren<SpriteRenderer>();
            originalColors = new Color[childRenderers.Length];
            for (int i = 0; i < childRenderers.Length; i++)
            {
                originalColors[i] = childRenderers[i].color;
            }
        }

        if (wingBurstPoint == null) wingBurstPoint = transform;

        UpdateUI(true); // آپدیت فوری بدون انیمیشن در شروع
    }

    // ===================== CORE LOGIC =====================

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        // چک کردن زمان مصونیت (iFrames)
        if (Time.time < lastDamageTime + invincibilityDuration) return;

        lastDamageTime = Time.time;
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, maxHealth);

        // اجرای افکت‌ها و رویدادها
        PlayDamageEffects();
        UpdateUI(false);
        OnHealthChanged?.Invoke(CurrentHealth / maxHealth);
        OnTakeDamage?.Invoke();

        Debug.Log($"<color=orange>Damage Taken: {amount}. Current: {CurrentHealth}</color>");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, maxHealth);
        UpdateUI(false);
        OnHealthChanged?.Invoke(CurrentHealth / maxHealth);
    }

    // ===================== DEATH LOGIC =====================

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Debug.Log("<color=red>PLAYER DIED</color>");
        OnDeath?.Invoke();

        // انیمیشن ساده مرگ (چرخش و کوچک شدن)
        if (characterModelRoot != null)
        {
            characterModelRoot.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            characterModelRoot.DORotate(new Vector3(0, 0, 180), 0.5f);
        }
        
        Destroy(gameObject, 1f); // یا غیرفعال کردن
    }

    // ===================== VISUALS & UI =====================

    private void UpdateUI(bool immediate)
    {
        float targetFill = CurrentHealth / maxHealth;

        // 1. آپدیت نوار اصلی (سریع)
        if (healthBarFill != null)
        {
            healthBarFill.DOKill();
            healthBarFill.DOFillAmount(targetFill, immediate ? 0 : 0.2f).SetEase(Ease.OutQuad);
            
            // تغییر رنگ بر اساس گرادینت (سبز -> زرد -> قرمز)
            healthBarFill.DOColor(healthColorGradient.Evaluate(targetFill), 0.2f);
        }

        // 2. آپدیت نوار روح (با تاخیر) - Chip Away Effect
        if (ghostHealthBarFill != null)
        {
            // اگر هیل کردیم، نوار روح سریع پر شود، اگر دمیج خوردیم با تاخیر کم شود
            if (targetFill > ghostHealthBarFill.fillAmount)
            {
                ghostHealthBarFill.fillAmount = targetFill; // پر شدن فوری در هیل
            }
            else
            {
                ghostTween.Kill();
                ghostTween = ghostHealthBarFill.DOFillAmount(targetFill, ghostBarSpeed)
                    .SetDelay(ghostBarDelay)
                    .SetEase(Ease.OutSine);
            }
        }
    }

    private void PlayDamageEffects()
    {
        // 1. شیک زدن نوار سلامتی (فقط UI تکان بخورد)
        if (healthBarFill != null)
            healthBarFill.transform.parent.DOShakePosition(0.3f, 5f, 10, 90f);

        // 2. انیمیشن فلش قرمز روی کاراکتر
        if (childRenderers != null && childRenderers.Length > 0)
        {
            Sequence flashSeq = DOTween.Sequence();
            
            // تغییر رنگ به قرمز
            flashSeq.AppendCallback(() => SetSpritesColor(damageFlashColor));
            flashSeq.AppendInterval(0.1f);
            // برگشت به رنگ اصلی
            flashSeq.AppendCallback(ResetSpritesColor);
            
            // تکرار برای تعداد مشخص
            flashSeq.SetLoops(flashCount); 
        }

        // 3. پارتیکل پر
        if (wingBurstPrefab != null)
        {
            var vfx = Instantiate(wingBurstPrefab, wingBurstPoint.position, Quaternion.identity);
            vfx.Play(wingBurstPoint.position);
        }
    }

    // متدهای کمکی برای تغییر رنگ اسپرایت‌ها
    private void SetSpritesColor(Color col)
    {
        foreach (var sr in childRenderers) 
            if(sr != null) sr.color = col;
    }

    private void ResetSpritesColor()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null)
                childRenderers[i].color = originalColors[i];
        }
    }

    private void OnDestroy()
    {
        // پاکسازی تمام انیمیشن‌ها برای جلوگیری از ارور
        transform.DOKill();
        if (healthBarFill != null) healthBarFill.DOKill();
        if (ghostHealthBarFill != null) ghostHealthBarFill.DOKill();
        if (characterModelRoot != null) characterModelRoot.DOKill();
    }
}
