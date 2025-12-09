using UnityEngine;
using System.Collections;

public class SeekMobility : EnemyMobility
{
    [Header("Seek Settings")]
    public float seekDuration = 3f;   // چند ثانیه دنبال کند
    public float restDuration = 2f;   // چند ثانیه مکث (مثلاً گشت‌زنی یا idle)
    public bool repeatSeek = true;    // آیا بعد از اتمام دوباره شروع کند

    private bool isSeeking = true;
    private float timer;

    private void Start()
    {
        timer = seekDuration;
    }

    public override void HandleMovement()
    {
        if (player == null) return;

        timer -= Time.deltaTime;

        if (isSeeking)
        {
            // 🔸 تعقیب پلیر
            Vector3 dir = (player.position - enemy.transform.position).normalized;
            enemy.transform.position += dir * moveSpeed * Time.deltaTime;

            if (timer <= 0)
            {
                // تایمر تموم شد → از حالت تعقیب خارج شو
                isSeeking = false;
                timer = restDuration;
            }
        }
        else
        {
            // 🔹 مرحله استراحت / توقف
            if (timer <= 0 && repeatSeek)
            {
                isSeeking = true;
                timer = seekDuration;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (player != null && isSeeking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(enemy.transform.position, player.position);
        }
    }
#endif
}