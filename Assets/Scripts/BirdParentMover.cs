using UnityEngine;

public class BirdParentMover : MonoBehaviour
{
    public float forwardSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector2.right * forwardSpeed * Time.deltaTime);
    }
}