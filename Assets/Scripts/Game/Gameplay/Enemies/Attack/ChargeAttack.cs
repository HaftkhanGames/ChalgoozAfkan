using UnityEngine;

public class ChargeAttack : EnemyAttack
{
    public float chargeSpeed = 10f;
    private bool charging;
    private float chargeEndTime;

    public override void HandleAttack()
    {
        if (player == null) return;

        if (!charging && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            charging = true;
            chargeEndTime = Time.time + 0.25f;
            Debug.Log($"{enemy.name} starts CHARGE attack!");
        }

        if (charging)
        {
            Vector3 dir = (player.position - enemy.transform.position).normalized;
            enemy.transform.position += dir * chargeSpeed * Time.deltaTime;

            if (Time.time > chargeEndTime)
            {
                charging = false;
                Debug.Log($"{enemy.name} ends CHARGE attack!");
            }
        }
    }
}