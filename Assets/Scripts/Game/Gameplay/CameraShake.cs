using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Cinemachine Impulse")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Shake Settings")]
    [SerializeField] private float defaultForce = 1f;
    [SerializeField] private Vector3 defaultVelocity = new Vector3(0, -1f, 0);

    private void Awake()
    {
        if (impulseSource == null)
            impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake()
    {
        Shake(defaultForce);
    }

    public void Shake(float force)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(defaultVelocity * force);
        }
        else
        {
            Debug.LogWarning("CameraShake: ImpulseSource is missing!");
        }
    }

    public void ShakeWithVelocity(Vector3 velocity)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(velocity);
        }
    }
}