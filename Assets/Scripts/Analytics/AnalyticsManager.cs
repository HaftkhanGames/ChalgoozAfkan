using UnityEngine;
using GameAnalyticsSDK; // فضای نام اصلی پکیج

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private void Awake()
    {
        // سینگلتون: مطمئن می‌شویم فقط یک نسخه وجود دارد
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // در تغییر صحنه‌ها باقی بماند
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // مقداردهی اولیه GameAnalytics
        GameAnalytics.Initialize();
        
        // (اختیاری) ثبت ایونت شروع شدن خود اپلیکیشن
        // GameAnalytics خودش Session Start را می‌زند، اما می‌توان لاگ کرد
        Debug.Log("GameAnalytics Initialized");
    }

    // ----------------------------------------------------------------
    // 1. مدیریت مراحل (Progression Events)
    // ----------------------------------------------------------------

    /// <summary>
    /// زمانی که مرحله شروع می‌شود
    /// </summary>
    /// <param name="levelNumber">شماره یا نام مرحله</param>
    public void LogLevelStart(int levelNumber)
    {
        // فرمت نام مرحله: Level_01
        string levelName = $"Level_{levelNumber:00}"; 
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
        Debug.Log($"Analytics: Start {levelName}");
    }

    /// <summary>
    /// زمانی که مرحله با موفقیت تمام می‌شود
    /// </summary>
    /// <param name="levelNumber">شماره مرحله</param>
    /// <param name="score">امتیاز کسب شده (اختیاری)</param>
    public void LogLevelComplete(int levelNumber, int score = 0)
    {
        string levelName = $"Level_{levelNumber:00}";
        // پارامتر سوم امتیاز است که اختیاری است
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName, score);
        Debug.Log($"Analytics: Complete {levelName} with Score: {score}");
    }

    /// <summary>
    /// زمانی که بازیکن در مرحله می‌بازد
    /// </summary>
    public void LogLevelFail(int levelNumber, int score = 0)
    {
        string levelName = $"Level_{levelNumber:00}";
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelName, score);
        Debug.Log($"Analytics: Fail {levelName}");
    }

    // ----------------------------------------------------------------
    // 2. مدیریت منابع و اقتصاد (Resource Events)
    // ----------------------------------------------------------------
    
    // در GameAnalytics دو نوع جریان داریم:
    // Source: بدست آوردن منبع (مثلا جایزه گرفتن)
    // Sink: خرج کردن منبع (مثلا خرید آیتم)

    /// <summary>
    /// ثبت خرج کردن پول یا الماس
    /// </summary>
    /// <param name="currencyType">نوع ارز (Coins, Gems, Stars)</param>
    /// <param name="amount">مقدار خرج شده</param>
    /// <param name="itemType">دسته بندی آیتم (مثلا Decoration, Booster)</param>
    /// <param name="itemId">آیدی دقیق آیتم (مثلا Tree_01)</param>
    public void LogResourceSpend(string currencyType, float amount, string itemType, string itemId)
    {
        // Sink یعنی خروجی (خرج کردن)
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, currencyType, amount, itemType, itemId);
        Debug.Log($"Analytics: Spent {amount} {currencyType} on {itemId}");
    }

    /// <summary>
    /// ثبت بدست آوردن پول یا الماس
    /// </summary>
    /// <param name="currencyType">نوع ارز (Coins, Gems)</param>
    /// <param name="amount">مقدار بدست آمده</param>
    /// <param name="sourceType">منبع درآمد (مثلا LevelReward, DailyBonus, ShopPack)</param>
    /// <param name="sourceId">آیدی دقیق منبع (مثلا Level_05)</param>
    public void LogResourceGain(string currencyType, float amount, string sourceType, string sourceId)
    {
        // Source یعنی ورودی (درآمد)
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, currencyType, amount, sourceType, sourceId);
        Debug.Log($"Analytics: Gained {amount} {currencyType} from {sourceType}");
    }

    // ----------------------------------------------------------------
    // 3. ایونت‌های طراحی (Design Events - Custom)
    // ----------------------------------------------------------------

    /// <summary>
    /// هر نوع ایونت خاص دیگری که می‌خواهید ترک کنید
    /// </summary>
    /// <param name="eventId">ساختار سلسله مراتبی با دو نقطه (مثلا UI:Click:Settings)</param>
    /// <param name="value">مقدار عددی اختیاری</param>
    public void LogDesignEvent(string eventId, float value = 0)
    {
        GameAnalytics.NewDesignEvent(eventId, value);
    }
    
    // ----------------------------------------------------------------
    // 4. ایونت‌های خطا (Error Events)
    // ----------------------------------------------------------------
    
    public void LogError(string message)
    {
        GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, message);
    }
}
