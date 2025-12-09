using UnityEngine;

public class StaticHoverMobility : EnemyMobility
{
    public float orbitRadius = 2f;
    public float waveAmplitude = 1f;
    public float waveFrequency = 1f;

    public override void HandleMovement()
    {
        Vector3 hoverOffset = new Vector3(0, Mathf.Sin(Time.time * waveFrequency) * waveAmplitude, 0);
        enemy.transform.position = startPos + hoverOffset;
    }
}