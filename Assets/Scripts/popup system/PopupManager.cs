using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Prefabs")]
    [Tooltip("پریفب پیام موفقیت آمیز")]
    public GameObject successPrefab;
    
    [Tooltip("پریفب پیام خطا یا شکست")]
    public GameObject failurePrefab;

    [Header("Settings")]
    [Tooltip("محل قرارگیری پاپ آپ ها در کنواس")]
    public Transform popupContainer; 

    private void Awake()
    {
        // الگوی سینگلتون
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // --- تابع نمایش موفقیت ---
    public void ShowSuccess(string message, float duration = 2.0f)
    {
        CreatePopup(successPrefab, message, duration);
    }

    // --- تابع نمایش خطا/شکست ---
    public void ShowError(string message, float duration = 2.0f)
    {
        CreatePopup(failurePrefab, message, duration);
    }

    // منطق اصلی ساخت و مقداردهی
    private void CreatePopup(GameObject prefab, string msg, float time)
    {
        if (prefab == null || popupContainer == null)
        {
            Debug.LogError("PopupManager: Prefab or Container is missing!");
            return;
        }

        // ساخت نمونه جدید از پریفب
        GameObject popupObj = Instantiate(prefab, popupContainer);
        
        // دریافت اسکریپت و تنظیم متن و زمان
        PopupMessage popupScript = popupObj.GetComponent<PopupMessage>();
        if (popupScript != null)
        {
            popupScript.Setup(msg, time);
        }
        else
        {
            Debug.LogWarning("Popup Prefab does not have 'PopupMessage' script attached!");
            // اگر اسکریپت نداشت، حداقل بعد از تایم حذف شود
            Destroy(popupObj, time); 
        }
    }
}