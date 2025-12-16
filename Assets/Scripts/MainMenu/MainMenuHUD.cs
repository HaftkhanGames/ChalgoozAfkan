using UnityEngine;
using RTLTMPro; // فضای نام پکیج RTL
using System;

public class MainMenuHUD : MonoBehaviour
{
    public static MainMenuHUD Instance;

    [Header("Text References (RTLTMPro)")]
    // نوع متغیرها را به RTLTextMeshPro تغییر دادیم
    public RTLTextMeshPro coinText;
    public RTLTextMeshPro gemText;
    public RTLTextMeshPro starText;
    public RTLTextMeshPro heartText;
    public RTLTextMeshPro heartTimerText;

    [Header("Settings")]
    public string fullHeartText = "پر"; // متن فارسی

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshAllUI();
    }

    private void Update()
    {
        UpdateHeartTimer();
    }

    public void RefreshAllUI()
    {
        if (GamePersistenceManager.Instance == null) return;
        
        PlayerData data = GamePersistenceManager.Instance.data;

        // تبدیل اعداد به رشته، سپس تبدیل به ارقام فارسی
        if (coinText) coinText.text = ToPersianNumber(data.coins.ToString("N0"));
        if (gemText) gemText.text = ToPersianNumber(data.gems.ToString("N0"));
        if (starText) starText.text = ToPersianNumber(data.stars.ToString("N0"));
        
        UpdateHeartCountUI();
    }

    public void UpdateHeartCountUI()
    {
        if (GamePersistenceManager.Instance == null) return;
        
        PlayerData data = GamePersistenceManager.Instance.data;
        int maxHearts = GamePersistenceManager.Instance.maxHearts;

        if (heartText) 
        {
            // فرمت: ۵/۵
            string text = $"{data.currentHearts}/{maxHearts}";
            heartText.text = ToPersianNumber(text);
        }
    }

    private void UpdateHeartTimer()
    {
        if (GamePersistenceManager.Instance == null || heartTimerText == null) return;

        double secondsLeft = GamePersistenceManager.Instance.GetSecondsToNextHeart();

        if (secondsLeft > 0)
        {
            TimeSpan t = TimeSpan.FromSeconds(secondsLeft);
            // فرمت انگلیسی زمان
            string timeString = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            // نمایش به صورت فارسی: ۱۴:۳۰
            heartTimerText.text = ToPersianNumber(timeString);
        }
        else
        {
            if (GamePersistenceManager.Instance.data.currentHearts >= GamePersistenceManager.Instance.maxHearts)
            {
                heartTimerText.text = fullHeartText;
            }
            else
            {
                // زمان تمام شد اما رفرش انجام نشده، موقتاً صفر نشان بده تا رفرش شود
                heartTimerText.text = ToPersianNumber("00:00");
                UpdateHeartCountUI();
            }
        }
    }

    // --- تابع کمکی تبدیل اعداد انگلیسی به فارسی ---
    public string ToPersianNumber(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        // جایگزینی کاراکتر به کاراکتر
        return input.Replace('0', '۰')
                    .Replace('1', '۱')
                    .Replace('2', '۲')
                    .Replace('3', '۳')
                    .Replace('4', '۴')
                    .Replace('5', '۵')
                    .Replace('6', '۶')
                    .Replace('7', '۷')
                    .Replace('8', '۸')
                    .Replace('9', '۹');
    }
}
