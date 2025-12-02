using UnityEngine;

public abstract class EnemyAttackSO : ScriptableObject
{
    public abstract void ExecuteAttack(EnemyBase enemy);
}