using UnityEngine;

public class BirdLaneMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("تعداد لاین‌های افقی")]
    public int totalLanesX = 3;
    [Tooltip("تعداد لاین‌های عمودی")]
    public int totalLanesY = 3;
    
    [Tooltip("فاصله بین هر لاین در محور ایکس")]
    public float laneDistanceX = 2.5f;
    [Tooltip("فاصله بین هر لاین در محور وای")]
    public float laneDistanceY = 2.5f;

    [Header("Movement Smoothness")]
    [Tooltip("سرعت جابجایی بین لاین‌ها")]
    public float moveSpeed = 15f;
    [Tooltip("مقدار کج شدن پرنده هنگام حرکت به چپ و راست")]
    public float tiltAmount = 20f;
    [Tooltip("سرعت کج شدن و برگشتن به حالت عادی")]
    public float tiltSpeed = 10f;

    [Header("Input Settings")]
    public float minSwipeDistance = 30f; // کمتر شد تا حساس‌تر باشد
    public bool useMouseForTesting = true; // برای تست راحت در ادیتور

    // وضعیت فعلی
    private int currentLaneX;
    private int currentLaneY;
    private Vector3 targetLocalPosition;
    
    // متغیرهای ورودی
    private Vector2 touchStartPos;
    private bool isSwiping = false;

    void Start()
    {
        // محاسبه لاین وسط به عنوان نقطه شروع
        currentLaneX = totalLanesX / 2;
        currentLaneY = totalLanesY / 2;

        UpdateTargetPosition();
        
        // ست کردن پوزیشن اولیه بدون انیمیشن
        transform.localPosition = targetLocalPosition;
    }

    void Update()
    {
        HandleInput();
        MoveBird();
        HandleTilt();
    }

    /// <summary>
    /// مدیریت ورودی‌ها (تاچ و موس) با واکنش سریع
    /// </summary>
    void HandleInput()
    {
        // 1. تشخیص شروع تاچ یا کلیک
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }

        // 2. تشخیص ادامه حرکت (تاچ یا موس)
        if (Input.GetMouseButton(0) && isSwiping)
        {
            Vector2 currentSwipe = (Vector2)Input.mousePosition - touchStartPos;

            // اگر طول حرکت از حداقل بیشتر شد، فرمان را اجرا کن
            if (currentSwipe.magnitude >= minSwipeDistance)
            {
                ProcessSwipe(currentSwipe);
                isSwiping = false; // جلوگیری از تشخیص مجدد در یک بار کشیدن
            }
        }

        // 3. پایان تاچ (ریست کردن برای اطمینان)
        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }
    }

    /// <summary>
    /// پردازش جهت سوایپ و اعمال حرکت
    /// </summary>
    void ProcessSwipe(Vector2 swipeVector)
    {
        // تشخیص افقی یا عمودی بودن حرکت
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            // حرکت افقی
            if (swipeVector.x > 0) ChangeLaneX(1);  // Right
            else ChangeLaneX(-1); // Left
        }
        else
        {
            // حرکت عمودی
            if (swipeVector.y > 0) ChangeLaneY(1);  // Up
            else ChangeLaneY(-1); // Down
        }
    }

    /// <summary>
    /// تغییر لاین افقی
    /// </summary>
    void ChangeLaneX(int direction)
    {
        int targetLane = currentLaneX + direction;
        
        // بررسی محدودیت‌ها (Clamp)
        if (targetLane >= 0 && targetLane < totalLanesX)
        {
            currentLaneX = targetLane;
            UpdateTargetPosition();
        }
    }

    /// <summary>
    /// تغییر لاین عمودی
    /// </summary>
    void ChangeLaneY(int direction)
    {
        int targetLane = currentLaneY + direction;

        // بررسی محدودیت‌ها (Clamp)
        if (targetLane >= 0 && targetLane < totalLanesY)
        {
            currentLaneY = targetLane;
            UpdateTargetPosition();
        }
    }

    /// <summary>
    /// محاسبه پوزیشن مقصد بر اساس شماره لاین‌ها
    /// </summary>
    void UpdateTargetPosition()
    {
        // فرمول: (شماره لاین - وسط) * فاصله
        // مثال: اگر 3 لاین باشد، وسط می‌شود 1. لاین 0 می‌شود -1 * فاصله.
        float targetX = (currentLaneX - (totalLanesX / 2)) * laneDistanceX;
        float targetY = (currentLaneY - (totalLanesY / 2)) * laneDistanceY;

        // حفظ مقدار Z فعلی (چون پرنده در طول حرکت می‌کند)
        targetLocalPosition = new Vector3(targetX, targetY, transform.localPosition.z);
    }

    /// <summary>
    /// حرکت نرم به سمت پوزیشن مقصد
    /// </summary>
    void MoveBird()
    {
        // استفاده از Vector3.Lerp برای نرمی حرکت
        // برای حرکت در راستای Z (جلو رفتن)، معمولاً پرنت حرکت می‌کند و این اسکریپت فقط X و Y لوکال را تغییر می‌دهد
        // اما برای اطمینان Z را جدا می‌کنیم یا کل لوکال را ست می‌کنیم.
        
        Vector3 newPos = Vector3.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * moveSpeed);
        
        // نکته مهم: اگر بازی رانر است و این آبجکت حرکت رو به جلو دارد، نباید Z لوکال را دستکاری کنیم مگر اینکه ثابت باشد.
        // اینجا فرض بر این است که Z لوکال ثابت است و حرکت رو به جلو توسط Parent انجام می‌شود.
        transform.localPosition = newPos; 
    }

    /// <summary>
    /// جلوه بصری چرخش (Tilt) هنگام حرکت به چپ و راست
    /// </summary>
    void HandleTilt()
    {
        float targetRotZ = 0;
        
        // اگر پوزیشن فعلی با مقصد فاصله زیادی در محور X دارد، یعنی در حال حرکت هستیم
        float diffX = targetLocalPosition.x - transform.localPosition.x;

        // اگر اختلاف مثبت باشد (حرکت به راست) -> چرخش منفی (ساعتگرد) و برعکس
        // آستانه 0.1 برای جلوگیری از لرزش
        if (Mathf.Abs(diffX) > 0.1f)
        {
            // محاسبه میزان چرخش بر اساس جهت حرکت (-1 برای راست، 1 برای چپ تا حس طبیعی بدهد)
            targetRotZ = Mathf.Sign(diffX) * -tiltAmount; 
        }

        // اعمال چرخش نرم
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotZ);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}
