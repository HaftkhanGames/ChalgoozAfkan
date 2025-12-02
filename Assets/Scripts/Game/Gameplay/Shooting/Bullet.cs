using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public float downwardForce = -2f;
    public float lifeTime = 3f;

    private Vector3 direction;

    void Start()
    {
        direction = new Vector3(1f, downwardForce, 0f).normalized;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
