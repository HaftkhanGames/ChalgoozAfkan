using UnityEngine;

public class BirdShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float cooldown = 0.2f;

    private float nextShootTime;
    public Animator birdAnimator;
    public FaceExpressionSystem faceExpressionSystem;
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
        faceExpressionSystem.ShowExpression(FaceExpressionSystem.ExpressionType.Attack,0.4f);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        birdAnimator.Play("Attack");
    }
}
