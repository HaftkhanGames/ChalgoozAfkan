using UnityEngine;

public class LinearPatrolMobility : EnemyMobility
{
    [Header("Patrol Settings")]
    public float patrolDistance = 4f;
    public bool vertical = false;   // اگر تیک بخورد → حرکت عمودی، در غیر این صورت افقی

    private bool movingToEnd = true;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 direction;

    private void Start()
    {
        startPoint = startPos;
        endPoint = vertical
            ? startPoint + Vector3.up * patrolDistance
            : startPoint + Vector3.right * patrolDistance;

        direction = (endPoint - startPoint).normalized;
    }

    public override void HandleMovement()
    {
        if (enemy == null) return;

        // حرکت به سمت مقصد فعلی
        Vector3 target = movingToEnd ? endPoint : startPoint;
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        // اگر به نقطه مقصد رسید، جهت را برگردان
        if (Vector3.Distance(enemy.transform.position, target) <= 0.05f)
        {
            movingToEnd = !movingToEnd;
        }
    }
}