using UnityEngine;
using System.Collections;

public class WaveFormAttack : EnemyAttack
{
    public float interval = 3f;
    public float waveRadius = 3f;
    public float pushForce = 5f;
    private float nextWave;

    public override void HandleAttack()
    {
        if (Time.time >= nextWave)
        {
            nextWave = Time.time + interval;
            StartCoroutine(EmitWave());
        }
    }

    private IEnumerator EmitWave()
    {
        // موج انرژی (می‌تونی Particle یا Scale Animation بزاری)
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, waveRadius);
        foreach (var h in hits)
        {
            if (h.CompareTag("Player"))
            {
                Rigidbody2D rb = h.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    Vector2 dir = (h.transform.position - enemy.transform.position).normalized;
                    rb.AddForce(dir * pushForce, ForceMode2D.Impulse);
                }
            }
        }
        yield return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0.6f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, waveRadius);
    }
#endif
}