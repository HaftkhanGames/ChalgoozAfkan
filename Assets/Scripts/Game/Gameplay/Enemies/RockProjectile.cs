using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;
    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;
        
        // حرکت خطی
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void Setup(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        isActive = true;
        Debug.Log($"Projectile Setup: Direction={direction}, Speed={speed}");
    
        Destroy(gameObject, 5f);
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit: {other.name}");
        
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(10);
            }
        }
        
        Destroy(gameObject);
    }
}