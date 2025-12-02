using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Core References")]
    public EnemyMobility mobility;   // محور حرکت
    public EnemyAttack attack;       // محور حمله
    public Transform player;         // هدف بازیکن

    [Header("Stats")]
    public float health = 100f;

    protected virtual void Start()
    {
        // گرفتن رفرنس بازیکن
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // مقداردهی اولیه محور‌ها
        mobility?.Initialize(this);
        attack?.Initialize(this);
    }

    protected virtual void Update()
    {
        // اجرای رفتارهای ماژولار
        mobility?.HandleMovement();
        attack?.HandleAttack();
    }
}