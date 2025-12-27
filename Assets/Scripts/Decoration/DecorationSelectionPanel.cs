using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecorationSelectionPanel : MenuPanel 
{
    public static DecorationSelectionPanel Instance;

    [Header("Components")]
    public Transform container;          // آبجکت Content داخل ScrollView
    public VariantItemUI itemPrefab;     // پریفب ساخته شده در مرحله ۱
    public Button confirmButton;

    private DecorationSpotController currentSpot;
    private int selectedVariantIndex = 0;
    
    // لیستی برای نگه داشتن دکمه‌های ساخته شده (برای مدیریت هایلایت)
    private List<VariantItemUI> spawnedItems = new List<VariantItemUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (confirmButton)
            confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    // --- تابع اصلی: فراخوانی توسط TaskItemUI یا سیستم ---
    public void OpenSelection(DecorationSpotController spot)
    {
        currentSpot = spot;
        selectedVariantIndex = 0; // پیش‌فرض اولین آیتم انتخاب است

        // 1. پاک کردن آیتم‌های قبلی لیست
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
        spawnedItems.Clear();

        // 2. خواندن لیست Variants از SO شما
        var variantList = spot.itemData.variants; 

        if (variantList == null || variantList.Count == 0)
        {
            Debug.LogError("No variants data found in SO!");
            return;
        }

        // 3. حلقه برای ساخت دکمه‌ها
        for (int i = 0; i < variantList.Count; i++)
        {
            // ساخت آبجکت
            VariantItemUI newItem = Instantiate(itemPrefab, container);
            
            // دریافت آیکون از کلاس DecorationVariantData شما
            Sprite icon = variantList[i].thumbnail; 
            
            // مقداردهی اولیه (اگر i برابر 0 باشد یعنی انتخاب شده است)
            newItem.Initialize(i, icon, i == 0, OnItemSelected);
            
            spawnedItems.Add(newItem);
        }

        // 4. نمایش پنل و پیش‌نمایش مدل اول
        base.Open();
        currentSpot.PreviewModel(0);
    }

    // وقتی روی یکی از آیتم‌ها کلیک شد
    private void OnItemSelected(int index)
    {
        selectedVariantIndex = index;

        // آپدیت گرافیک دکمه‌ها (فقط دکمه جدید هایلایت شود)
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            spawnedItems[i].SetSelectionState(i == selectedVariantIndex);
        }

        // تغییر مدل سه بعدی در صحنه
        if (currentSpot != null)
        {
            currentSpot.PreviewModel(selectedVariantIndex);
        }
    }

    // نهایی سازی
    private void OnConfirmClicked()
    {
        if (currentSpot != null)
        {
            // currentSpot.ConfirmAndFinish(selectedVariantIndex);
        }
        Close();
    }
}
