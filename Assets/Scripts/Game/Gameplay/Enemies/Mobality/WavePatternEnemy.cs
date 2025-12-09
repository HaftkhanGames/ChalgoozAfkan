using UnityEngine;

public class WavePatternMobility : EnemyMobility
{
    public float waveLength = 3f;
    public float waveAmplitude = 1f;
    public float waveFrequency = 1f;
    public override void HandleMovement()
    {
        float x = Mathf.Sin(Time.time * moveSpeed / waveLength);
        float y = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        enemy.transform.position = startPos + new Vector3(x * waveLength, y, 0);
    }
}