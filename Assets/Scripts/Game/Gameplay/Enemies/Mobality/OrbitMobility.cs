using UnityEngine;

public class OrbitMobility : EnemyMobility
{
    public bool orbitAroundPlayer = false;
    public float orbitRadius = 2f;

    public override void HandleMovement()
    {
        Transform target = orbitAroundPlayer && player != null ? player : enemy.transform;
        float angle = Time.time * moveSpeed;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbitRadius;

        enemy.transform.position = (orbitAroundPlayer && player != null ? player.position : startPos) + offset;
    }
}