using UnityEngine;
using DG.Tweening; // اضافه کردن نیم‌اسپیس DOTween

[RequireComponent(typeof(CanvasGroup))]
public class MenuPanel : MonoBehaviour
{
    public MenuPanelType panelType;

    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public Ease openEase = Ease.OutBack; // حالت فنری جذاب برای باز شدن
    public Ease closeEase = Ease.InBack; // حالت معکوس برای بسته شدن
    public float startYOffset = 1000f;   // فاصله‌ای که پنل از بالا میاد

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        
        // ذخیره پوزیشن اصلی (معمولا 0,0 وسط صفحه)
        originalPosition = rectTransform.anchoredPosition;
    }

    public void Open()
    {
        // 1. فعال کردن آبجکت
        gameObject.SetActive(true);
        
        // 2. ریست کردن و قطع انیمیشن‌های قبلی (برای جلوگیری از تداخل)
        rectTransform.DOKill();
        canvasGroup.DOKill();

        // 3. تنظیم وضعیت اولیه (بالای صفحه و شفاف)
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + startYOffset);

        // 4. انیمیشن حرکت به پایین (به جای اصلی)
        rectTransform.DOAnchorPosY(originalPosition.y, animationDuration)
            .SetEase(openEase)
            .SetUpdate(true); // کار کردن حتی اگر بازی Pause باشد

        // 5. انیمیشن ظاهر شدن (Fade In)
        canvasGroup.DOFade(1f, animationDuration * 0.8f)
            .SetUpdate(true);
            
        // فعال کردن اینتراکشن (کلیک)
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        // قطع انیمیشن‌های قبلی
        rectTransform.DOKill();
        canvasGroup.DOKill();

        // غیرفعال کردن کلیک بلافاصله تا کاربر نتونه اسپم کنه
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // انیمیشن محو شدن
        canvasGroup.DOFade(0f, animationDuration * 0.5f)
            .SetUpdate(true);

        // انیمیشن رفتن به بالا (یا پایین - اینجا میره بالا که انگار برمیگرده)
        rectTransform.DOAnchorPosY(originalPosition.y + startYOffset, animationDuration)
            .SetEase(closeEase)
            .SetUpdate(true)
            .OnComplete(() => 
            {
                // وقتی انیمیشن تموم شد، آبجکت رو غیرفعال کن
                gameObject.SetActive(false);
            });
    }
    
    // متدی برای بستن فوری بدون انیمیشن (برای شروع بازی)
    public void CloseImmediate()
    {
        gameObject.SetActive(false);
        if(canvasGroup) canvasGroup.alpha = 0;
    }
}
