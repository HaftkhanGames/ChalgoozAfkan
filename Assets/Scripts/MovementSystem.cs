using UnityEngine;

public class BirdLaneMovement : MonoBehaviour
{
    [Header("Vertical Lanes (Y Axis)")]
    public int totalLanesY = 3;
    public float laneDistanceY = 2.5f;
    public float switchSpeedY = 10f;

    [Header("Horizontal Lanes (X Axis)")]
    public int totalLanesX = 3;
    public float laneDistanceX = 2.5f;
    public float switchSpeedX = 10f;

    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;  // pixel

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    private int currentLaneY;
    private int currentLaneX;

    private float targetLocalY;
    private float targetLocalX;

    void Start()
    {
        currentLaneY = totalLanesY / 2;
        currentLaneX = totalLanesX / 2;

        targetLocalY = GetLaneY(currentLaneY);
        targetLocalX = GetLaneX(currentLaneX);
    }

    void Update()
    {
        HandleSwipe();

        Vector3 local = transform.localPosition;
        local.y = Mathf.Lerp(local.y, targetLocalY, Time.deltaTime * switchSpeedY);
        local.x = Mathf.Lerp(local.x, targetLocalX, Time.deltaTime * switchSpeedX);
        transform.localPosition = local;
    }

    void HandleSwipe()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStartPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            touchEndPos = touch.position;

            Vector2 swipe = touchEndPos - touchStartPos;
            if (swipe.magnitude < minSwipeDistance) return;

            // Horizontal or vertical?
            if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
            {
                if (swipe.x > 0) MoveRight();
                else MoveLeft();
            }
            else
            {
                if (swipe.y > 0) MoveUp();
                else MoveDown();
            }
        }
    }

    void MoveUp()
    {
        if (currentLaneY < totalLanesY - 1)
        {
            currentLaneY++;
            targetLocalY = GetLaneY(currentLaneY);
        }
    }

    void MoveDown()
    {
        if (currentLaneY > 0)
        {
            currentLaneY--;
            targetLocalY = GetLaneY(currentLaneY);
        }
    }

    void MoveLeft()
    {
        if (currentLaneX > 0)
        {
            currentLaneX--;
            targetLocalX = GetLaneX(currentLaneX);
        }
    }

    void MoveRight()
    {
        if (currentLaneX < totalLanesX - 1)
        {
            currentLaneX++;
            targetLocalX = GetLaneX(currentLaneX);
        }
    }

    float GetLaneY(int lane)
    {
        int mid = totalLanesY / 2;
        return (lane - mid) * laneDistanceY;
    }

    float GetLaneX(int lane)
    {
        int mid = totalLanesX / 2;
        return (lane - mid) * laneDistanceX;
    }
}
