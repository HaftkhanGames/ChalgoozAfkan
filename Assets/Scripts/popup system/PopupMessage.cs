using UnityEngine;
using RTLTMPro; // برای پشتیبانی از متن فارسی

public class PopupMessage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RTLTextMeshPro messageText;

    // این تابع توسط منیجر صدا زده می‌شود
    public void Setup(string text, float duration)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }

        // دستور نابودی خودکار بعد از duration ثانیه
        Destroy(gameObject, duration);
    }
}