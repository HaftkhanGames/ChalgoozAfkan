using UnityEngine;

public class BirdWingBurstVFX : MonoBehaviour
{
    [Header("Wing Prefabs")]
    public WingPieceVFX[] wingPrefabs;

    [Header("Burst Settings")]
    public int minPieces = 5;
    public int maxPieces = 8;
    public float forceMin = 2f;
    public float forceMax = 5f;

    public void Play(Vector3 position)
    {
        Debug.Log("WingBurstVFX.Play ✅");

        int count = Random.Range(minPieces, maxPieces + 1);

        for (int i = 0; i < count; i++)
        {
            WingPieceVFX wing =
                Instantiate(
                    wingPrefabs[Random.Range(0, wingPrefabs.Length)],
                    position,
                    Quaternion.Euler(0, 0, Random.Range(0, 360f))
                );

            Vector2 dir = Random.insideUnitCircle.normalized;
            float force = Random.Range(forceMin, forceMax);

            wing.Launch(dir * force);
        }
    }
}