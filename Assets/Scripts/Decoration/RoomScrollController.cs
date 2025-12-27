using UnityEngine;
using DG.Tweening; // برای انیمیشن حرکت دوربین

public class RoomScrollController : MonoBehaviour
{
    public static RoomScrollController Instance;

    [Header("Settings")]
    public SpriteRenderer backgroundSprite;
    public bool startAtCenter = true;

    private Camera cam;
    private Vector3 dragOrigin;
    private float minX, maxX;
    private bool canScroll = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        cam = Camera.main;
    }

    private void Start()
    {
        CalculateCameraBounds();
        if (startAtCenter) CenterCamera();
    }

    private void LateUpdate()
    {
        if (backgroundSprite == null || !canScroll) return;
        HandleInput();
    }

    // --- متد جدید برای حرکت اتوماتیک دوربین ---
    public void FocusOnTarget(Transform targetTransform, float duration = 1f)
    {
        if (targetTransform == null) return;

        // محاسبه X هدف با رعایت محدودیت‌های اتاق (Clamp)
        float targetX = Mathf.Clamp(targetTransform.position.x, minX, maxX);

        // قطع کردن هر انیمیشن قبلی روی دوربین
        cam.transform.DOKill();

        // حرکت نرم به سمت هدف
        cam.transform.DOMoveX(targetX, duration).SetEase(Ease.OutCubic);
    }

    private void CalculateCameraBounds()
    {
        if (backgroundSprite == null) return;

        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float bgWidth = backgroundSprite.bounds.size.x;
        float bgCenterX = backgroundSprite.transform.position.x;

        if (bgWidth <= camWidth)
        {
            minX = bgCenterX;
            maxX = bgCenterX;
            canScroll = false;
            return;
        }

        minX = backgroundSprite.bounds.min.x + (camWidth / 2);
        maxX = backgroundSprite.bounds.max.x - (camWidth / 2);
    }

    private void CenterCamera()
    {
        Vector3 pos = cam.transform.position;
        pos.x = backgroundSprite.bounds.center.x;
        cam.transform.position = pos;
    }

    private void HandleInput()
    {
        // اگر تاچ کرد، انیمیشن اتوماتیک (Focus) را قطع کن تا کاربر کنترل را به دست بگیرد
        if (Input.GetMouseButtonDown(0))
        {
            cam.transform.DOKill();
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentPos;
            Vector3 moveVector = new Vector3(difference.x, 0, 0);
            
            cam.transform.position += moveVector;

            Vector3 clampedPos = cam.transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            cam.transform.position = clampedPos;
        }
    }
}
