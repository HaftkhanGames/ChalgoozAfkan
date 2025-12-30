using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

// تغییر مهم: ارث‌بری از MenuPanel حذف شد و تبدیل به MonoBehaviour شد
public class DecorationSelectionPanel : MonoBehaviour 
{
    public static DecorationSelectionPanel Instance;

    [Header("Components")]
    public Transform container;          
    public VariantItemUI itemPrefab;     
    public Button confirmButton;
    public RectTransform panelContent; // پنل اصلی برای انیمیشن حرکت
    public CanvasGroup canvasGroup;    // برای انیمیشن فید

    private DecorationSpotController currentSpot;
    private int selectedVariantIndex = 0;
    private List<VariantItemUI> spawnedItems = new List<VariantItemUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (confirmButton)
            confirmButton.onClick.AddListener(OnConfirmClicked);

        if(canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        // اطمینان از خاموش بودن در شروع بازی
        gameObject.SetActive(false);
    }

    public void OpenSelection(DecorationSpotController spot)
    {
        currentSpot = spot;
        selectedVariantIndex = 0; 

        // 1. پاکسازی آیتم‌های قبلی
        foreach (Transform child in container) Destroy(child.gameObject);
        spawnedItems.Clear();

        // 2. خواندن لیست Variants از دیتای اسپات
        var variantList = spot.itemData.variants; 
        if (variantList == null || variantList.Count == 0) return;

        // 3. ساخت دکمه‌ها
        for (int i = 0; i < variantList.Count; i++)
        {
            VariantItemUI newItem = Instantiate(itemPrefab, container);
            Sprite icon = variantList[i].thumbnail; 
            
            newItem.Initialize(i, icon, false, OnItemSelected);
            spawnedItems.Add(newItem);
        }

        // 4. نمایش پنل با انیمیشن
        AnimateEntry();

        // 5. انتخاب خودکار اولین مدل
        OnItemSelected(0);
    }

    private void AnimateEntry()
    {
        gameObject.SetActive(true);

        // تنظیمات اولیه قبل از انیمیشن
        if (canvasGroup) canvasGroup.alpha = 0f;
        if (panelContent) panelContent.anchoredPosition = new Vector2(0, -150f); // کمی پایین‌تر

        // اجرای انیمیشن‌ها
        if (canvasGroup) canvasGroup.DOFade(1f, 0.4f);
        if (panelContent) panelContent.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnItemSelected(int index)
    {
        selectedVariantIndex = index;

        // به‌روزرسانی گرافیک دکمه‌ها (کدام انتخاب شده)
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            spawnedItems[i].SetSelectionState(i == selectedVariantIndex);
        }

        // نمایش پیش‌نمایش مدل در صحنه
        if (currentSpot != null)
        {
            currentSpot.PreviewModel(selectedVariantIndex);
        }
    }

    private void OnConfirmClicked()
    {
        // نهایی کردن خرید/ساخت
        if (currentSpot != null)
        {
            currentSpot.ConfirmAndFinish(selectedVariantIndex);
        }
        MainMenuManager.Instance.GoBack();
        // ClosePanel();
    }

    // متد بستن مستقل
    public void ClosePanel()
    {
        if (canvasGroup)
        {
            canvasGroup.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
