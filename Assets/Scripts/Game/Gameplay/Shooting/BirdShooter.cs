using UnityEngine;

public class BirdShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float cooldown = 0.2f;

    [Header("Visual References")]
    public Animator birdAnimator;
    public FaceExpressionSystem faceExpressionSystem;

    // متغیرهای داخلی
    private float nextShootTime;
    // کش کردن شناسه انیمیشن برای پرفورمنس بهتر
    private static readonly int AttackAnimId = Animator.StringToHash("Attack");

    void Update()
    {
        // ورودی کیبورد برای تست روی کامپیوتر
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryShoot();
        }
    }

    // پابلیک شده برای اتصال به دکمه UI در موبایل
    public void TryShoot()
    {
        if (Time.time < nextShootTime) return;

        nextShootTime = Time.time + cooldown;
        PerformShoot();
    }

    private void PerformShoot()
    {
        // 1. تغییر چهره به حالت حمله (با استفاده از متد جدید)
        if (faceExpressionSystem != null)
        {
            faceExpressionSystem.ShowAttack(0.4f);
        }

        // 2. پخش انیمیشن
        if (birdAnimator != null)
        {
            // پارامتر سوم (0f) باعث می‌شود انیمیشن از فریم صفر شروع شود
            // این برای وقتی که تند تند شلیک می‌کنیم عالی است
            birdAnimator.Play(AttackAnimId, -1, 0f);
        }

        // 3. شلیک گلوله
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}