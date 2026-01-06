using UnityEngine;
using UnityEngine.UI;
using RTLTMPro; 

public class TaskItemUI : MonoBehaviour
{
    [Header("UI References")]
    public RTLTextMeshPro taskNameText; 
    public RTLTextMeshPro costText; 
    public Button actionButton;

    public Image taskIcon;
    // متغیر کمکی برای نگهداری دکور مربوط به این تسک
    private DecorationSpotController targetSpot;

    public void Setup(DecorationSpotController spot)
    {
        targetSpot = spot;

        if (spot.itemData != null)
        {
            // تنظیم نام
            taskNameText.text = spot.itemData.itemName;
            taskIcon.sprite = spot.itemData.icon;
            // --- تغییر برای اعداد فارسی ---
            int cost = spot.itemData.star;
            
            // تبدیل عدد به رشته فارسی
            string persianCost = ToPersianNumber(cost.ToString());
            
            // نمایش: مثلا "۳ ستاره"
            costText.text = persianCost;
            // -----------------------------

            // بررسی موجودی برای رنگ متن
            int currentStars = GamePersistenceManager.Instance.data.stars;
            costText.color = (currentStars >= cost) ? Color.white : Color.red;
        }

        // تنظیم دکمه
        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => 
            {
                HandleTaskClick(spot);
            });
        }
    }

    private void HandleTaskClick(DecorationSpotController spot)
    {
        int cost = spot.itemData.star;
        int currentStars = GamePersistenceManager.Instance.data.stars;

        // 1. بررسی موجودی
        if (currentStars < cost)
        {
            Debug.Log("Not enough stars! (ستاره کافی نیست)");
            return;
        }
        
        MainMenuManager.Instance.GoBack();
        RoomScrollController.Instance.FocusOnTarget(spot.transform);
        MainMenuManager.Instance.OpenPanel(MenuPanelType.SelectDecor);
        DecorationSelectionPanel.Instance.OpenSelection(spot);
    }

    /// <summary>
    /// متد کمکی برای تبدیل اعداد انگلیسی به فارسی
    /// </summary>
    private string ToPersianNumber(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

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
