using UnityEngine;

public class EnergyBeamAttack : EnemyAttack
{
    [Header("Beam Settings")]
    public LineRenderer beamRenderer;
    public float beamDuration = 0.5f;
    private float beamEndTime;

    public override void HandleAttack()
    {
        if (player == null || beamRenderer == null) return;

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            beamEndTime = Time.time + beamDuration;

            beamRenderer.enabled = true;
            beamRenderer.SetPosition(0, enemy.transform.position);
            beamRenderer.SetPosition(1, player.position);

            Debug.Log($"{enemy.name} fires ENERGY BEAM!");

            // Physics2D.Raycast برای تشخیص برخورد با Player
        }

        if (beamRenderer.enabled && Time.time > beamEndTime)
            beamRenderer.enabled = false;
    }
}