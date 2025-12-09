using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LineStrikeAttack : EnemyAttack
{
    [Header("Line Strike Settings")]
    public bool vertical = false;        // جهت خط
    public float lineLength = 10f;       // طول خط
    public float telegraphDuration = 1.5f;
    public float lineWidth = 0.25f;
    public float debugDuration = 0.2f;   // مدت ماندن رنگ قرمز بعد از ضربه
    public LayerMask playerLayer;

    private bool isAttacking;
    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;
        lr.enabled = false;
    }

    public override void HandleAttack()
    {
        if (!isAttacking && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(PerformLineStrike());
        }
    }

    private IEnumerator PerformLineStrike()
    {
        isAttacking = true;

        Vector3 start = enemy.transform.position - (vertical ? Vector3.up : Vector3.right) * (lineLength / 2f);
        Vector3 end = enemy.transform.position + (vertical ? Vector3.up : Vector3.right) * (lineLength / 2f);

        // --- مرحله‌ی تلگراف (زرد):
        lr.enabled = true;
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        Debug.Log($"⚠️ {enemy.name} is telegraphing a LineStrike ({(vertical ? "Vertical" : "Horizontal")})");

        yield return new WaitForSeconds(telegraphDuration);

        // --- مرحله‌ی ضربه واقعی (قرمز):
        lr.startColor = Color.red;
        lr.endColor = Color.red;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            enemy.transform.position,
            vertical ? new Vector2(lineWidth, lineLength) : new Vector2(lineLength, lineWidth),
            0f,
            Vector2.zero,
            0f,
            playerLayer
        );

        bool hitSomething = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                hitSomething = true;
                Debug.Log($"💥 Hit: {hit.collider.name} by {enemy.name}'s LineStrike!");
                // hit.collider.GetComponent<HealthPoint>()?.TakeDamage(attackDamage);
            }
        }

        if (!hitSomething)
            Debug.Log($"❌ {enemy.name} LineStrike hit nothing.");

        yield return new WaitForSeconds(debugDuration);

        lr.enabled = false;
        isAttacking = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Vector3 size = vertical ? new Vector3(lineWidth, lineLength, 0) : new Vector3(lineLength, lineWidth, 0);
        Gizmos.DrawWireCube(transform.position, size);
    }
#endif
}
