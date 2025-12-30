using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening; // حتما برای حرکت نرم اضافه شود

// 1. تعریف کلاس تنظیمات برای جابجایی المان‌ها
[System.Serializable]
public class UIShifterConfig
{
    public string description;          // فقط برای توضیحات در اینسپکتور (مثلا: Move StartButton Left)
    public RectTransform targetElement; // دکمه یا المانی که باید حرکت کند
    public MenuPanelType triggerPanel;  // وقتی این پنل باز شد، حرکت انجام شود
    public float shiftOffsetX;          // مقدار جابجایی در محور X (منفی = چپ، مثبت = راست)
    public float animationDuration = 0.5f;
}

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [Header("Panels Reference")]
    public List<MenuPanel> allPanels;

    [Header("UI Shifting Settings")]
    // 2. لیست تنظیمات که در اینسپکتور پر می‌کنید
    public List<UIShifterConfig> uiShifters; 
    
    // دیکشنری برای ذخیره موقعیت اصلی المان‌ها تا بتوانیم برگردانیم
    private Dictionary<RectTransform, Vector2> initialPositions = new Dictionary<RectTransform, Vector2>();

    [Header("Settings")]
    public MenuPanelType startingPanel = MenuPanelType.MainMenu;

    private MenuPanel currentPanel;
    private Stack<MenuPanelType> panelHistory = new Stack<MenuPanelType>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // 3. ذخیره موقعیت اولیه تمام دکمه‌هایی که قرار است حرکت کنند
        foreach (var config in uiShifters)
        {
            if (config.targetElement != null && !initialPositions.ContainsKey(config.targetElement))
            {
                initialPositions.Add(config.targetElement, config.targetElement.anchoredPosition);
            }
        }

        // بستن همه پنل‌ها به صورت فوری در شروع
        foreach (var panel in allPanels)
        {
            panel.CloseImmediate();
        }
        
        // باز کردن پنل شروع
        OpenPanel(startingPanel);
    }

    public void OpenPanel(MenuPanelType type)
    {
        print(type.ToString());
        if (currentPanel != null && currentPanel.panelType == type) return;

        MenuPanel targetPanel = allPanels.Find(p => p.panelType == type);
        
        if (targetPanel != null)
        {
            if (currentPanel != null)
            {
                bool keepMainMenuOpen = (currentPanel.panelType == MenuPanelType.MainMenu);
                
                if (!keepMainMenuOpen)
                {
                    currentPanel.Close();
                }

                panelHistory.Push(currentPanel.panelType);
            }

            targetPanel.Open();
            currentPanel = targetPanel;

            // 4. فراخوانی تابع مدیریت جابجایی‌ها بر اساس پنل جدید
            UpdateUIShifters(type);
        }
        else
        {
            Debug.LogError($"Panel of type {type} not found!");
        }
    }

    public void GoBack()
    {
        if (panelHistory.Count > 0)
        {
            MenuPanelType previousType = panelHistory.Pop();
            
            if(currentPanel != null) currentPanel.Close();
            
            MenuPanel prevPanel = allPanels.Find(p => p.panelType == previousType);
            
            if(prevPanel != null)
            {
                if (!prevPanel.gameObject.activeInHierarchy)
                {
                    prevPanel.Open();
                }
                
                currentPanel = prevPanel;

                // 5. وقتی برمی‌گردیم عقب، باید وضعیت دکمه‌ها بر اساس پنل قبلی تنظیم شود
                UpdateUIShifters(prevPanel.panelType);
            }
        }
        else
        {
            if (currentPanel != null && currentPanel.panelType != MenuPanelType.MainMenu)
            {
                OpenPanel(MenuPanelType.MainMenu);
            }
            else
            {
                Debug.Log("Already at Main Menu root.");
            }
        }
    }

    // --- تابع جدید: مدیریت حرکت دکمه‌ها ---
    private void UpdateUIShifters(MenuPanelType activePanelType)
    {
        // الف) ابتدا همه المان‌ها را به حالت پیش‌فرض (Reset) برمی‌گردانیم
        // این کار باعث می‌شود اگر از پنل Tasks به Shop رفتیم، دکمه‌هایی که برای Tasks جابجا شده بودند برگردند
        foreach (var kvp in initialPositions)
        {
            RectTransform target = kvp.Key;
            Vector2 originalPos = kvp.Value;

            // چک می‌کنیم آیا این دکمه در حال حاضر باید جابجا باشد؟ اگر نه، برش گردان
            // (برای جلوگیری از تکرار انیمیشن، اگر سر جایش است کاری نمی‌کنیم)
            // اما بهتر است همیشه دستور حرکت به مکان اصلی را بدهیم (DOTween خودش هندل می‌کند)
            target.DOKill(); // قطع انیمیشن قبلی
            target.DOAnchorPos(originalPos, 0.4f).SetEase(Ease.OutQuad);
        }

        // ب) حالا بررسی می‌کنیم کدام دکمه‌ها برای پنلِ "فعلی" باید تغییر مکان دهند
        foreach (var config in uiShifters)
        {
            if (config.triggerPanel == activePanelType && config.targetElement != null)
            {
                // موقعیت اصلی را می‌گیریم
                Vector2 originalPos = initialPositions[config.targetElement];
                
                // موقعیت جدید با اعمال آفست
                Vector2 targetPos = new Vector2(originalPos.x + config.shiftOffsetX, originalPos.y);

                // انیمیشن به سمت موقعیت جدید (Override کردن حرکت Reset بالا)
                config.targetElement.DOKill();
                config.targetElement.DOAnchorPos(targetPos, config.animationDuration).SetEase(Ease.OutBack);
            }
        }
    }

    public void OnNavigateButton(string panelTypeName)
    {
        if (System.Enum.TryParse(panelTypeName, out MenuPanelType type))
        {
            OpenPanel(type);
        }
    }

    public void OnStartGameClicked()
    {
        if (GamePersistenceManager.Instance.TryConsumeHeart())
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            OpenPanel(MenuPanelType.NotEnoughHeart);
        }
    }
    
    public void OnBuyHeartClicked()
    {
        if(GamePersistenceManager.Instance.SpendCoins(500))
        {
            GoBack();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPanel != null && currentPanel.panelType != MenuPanelType.MainMenu)
            {
                GoBack();
            }
        }
    }
}
