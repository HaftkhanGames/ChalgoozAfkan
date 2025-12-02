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
        HandleInput();

        Vector3 local = transform.localPosition;

        local.y = Mathf.Lerp(local.y, targetLocalY, Time.deltaTime * switchSpeedY);
        local.x = Mathf.Lerp(local.x, targetLocalX, Time.deltaTime * switchSpeedX);

        transform.localPosition = local;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveUp();
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
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
