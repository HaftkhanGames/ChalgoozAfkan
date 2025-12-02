using UnityEngine;

public class ClusterMobility : EnemyMobility
{
    public Transform clusterCenter;
    public float cohesionRadius = 5f;

    public override void HandleMovement()
    {
        if (clusterCenter == null) return;

        Vector3 direction = (clusterCenter.position - enemy.transform.position).normalized;
        float distance = Vector3.Distance(clusterCenter.position, enemy.transform.position);

        if (distance > cohesionRadius)
            enemy.transform.position += direction * moveSpeed * Time.deltaTime;
        else
            enemy.transform.position += new Vector3(
                Mathf.Sin(Time.time * waveFrequency) * 0.1f,
                Mathf.Cos(Time.time * waveFrequency) * 0.1f,
                0
            ); // لرزش کوچک برای طبیعی بودن گروه
    }
}