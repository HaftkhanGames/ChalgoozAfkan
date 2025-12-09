using UnityEngine;

public class TeleportMobility : EnemyMobility
{
    private float nextTeleportTime;
    public float orbitRadius = 2f;
    public float teleportCooldown = 3f;

    public override void HandleMovement()
    {
        if (Time.time >= nextTeleportTime)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-orbitRadius, orbitRadius),
                Random.Range(-orbitRadius, orbitRadius),
                0
            );

            enemy.transform.position = startPos + randomOffset;
            nextTeleportTime = Time.time + teleportCooldown;
        }
    }
}