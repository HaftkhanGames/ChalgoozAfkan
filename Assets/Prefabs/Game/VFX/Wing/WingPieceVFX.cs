using UnityEngine;
using DG.Tweening;

public class WingPieceVFX : MonoBehaviour
{
    [Header("Movement")]
    public float gravity = 9f;
    public float rotateSpeedMin = -360f;
    public float rotateSpeedMax = 360f;

    [Header("Fade")]
    public float lifeTime = 1.5f;
    public float fadeTime = 0.5f;

    private Vector2 velocity;
    private float rotateSpeed;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Launch(Vector2 force)
    {
        velocity = force;
        rotateSpeed = Random.Range(rotateSpeedMin, rotateSpeedMax);

        // Fade out آخرش
        sr.DOFade(0f, fadeTime)
            .SetDelay(lifeTime - fadeTime)
            .OnComplete(() => Destroy(gameObject));
    }

    private void Update()
    {
        // گراویتی
        velocity.y -= gravity * Time.deltaTime;

        // حرکت
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // چرخش
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}