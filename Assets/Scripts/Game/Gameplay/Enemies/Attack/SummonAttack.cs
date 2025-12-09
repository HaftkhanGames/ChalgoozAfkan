using UnityEngine;

public class SummonAttack : EnemyAttack
{
    [Header("Summon Settings")]
    public GameObject minionPrefab;
    public int summonCount = 3;
    public float summonRadius = 3f;

    public override void HandleAttack()
    {
        if (minionPrefab == null) return;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            for (int i = 0; i < summonCount; i++)
            {
                Vector2 pos = (Vector2)enemy.transform.position +
                              Random.insideUnitCircle * summonRadius;

                GameObject.Instantiate(minionPrefab, pos, Quaternion.identity);
            }

            Debug.Log($"{enemy.name} SUMMONS {summonCount} minions!");
        }
    }
}