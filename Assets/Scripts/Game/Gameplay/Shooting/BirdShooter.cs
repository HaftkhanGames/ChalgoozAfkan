using UnityEngine;

public class BirdShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float cooldown = 0.2f;

    private float nextShootTime;
    public Animator birdAnimator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // موبایل ❗️ اگر خواستی بعداً تاچ اضافه می‌کنم
    }

    void Shoot()
    {
        if (Time.time < nextShootTime) return;

        nextShootTime = Time.time + cooldown;

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        birdAnimator.Play("Attack");
    }
}
