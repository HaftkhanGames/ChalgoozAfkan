using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [Header("Panels Reference")]
    public List<MenuPanel> allPanels; // تمام پنل‌ها رو بکش اینجا

    [Header("Settings")]
    public MenuPanelType startingPanel = MenuPanelType.MainMenu;

    private MenuPanel currentPanel;
    // استک برای دکمه بازگشت (Back) - اختیاری ولی برای تجربه کاربری عالیه
    private Stack<MenuPanelType> panelHistory = new Stack<MenuPanelType>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // بستن همه پنل‌ها
        foreach (var panel in allPanels)
        {
            panel.Close();
        }
        
        // باز کردن پنل شروع
        OpenPanel(startingPanel);
    }

    // --- Core Navigation ---

    public void OpenPanel(MenuPanelType type)
    {
        // PopupManager.Instance.ShowSuccess("موفقیت انجام شد!", 5f);
        // PopupManager.Instance.ShowError("سکه کافی نیست!", 3f);

        // اگر پنلی باز بود، ببندش (یا اگه میخوای روی هم باز بشن این خط رو بردار)
        if (currentPanel != null && currentPanel.panelType != MenuPanelType.MainMenu) 
        {
            currentPanel.Close();
        }

        MenuPanel targetPanel = allPanels.Find(p => p.panelType == type);
        
        if (targetPanel != null)
        {
            // اگر پنل اصلی نیست، پنل قبلی رو به تاریخچه اضافه کن (برای دکمه بک)
            if (currentPanel != null)
            {
                panelHistory.Push(currentPanel.panelType);
            }

            targetPanel.Open();
            currentPanel = targetPanel;
        }
        else
        {
            Debug.LogError($"Panel of type {type} not found in the list!");
        }
    }

    // دکمه بازگشت (مخصوصاً برای موبایل)
    public void GoBack()
    {
        if (panelHistory.Count > 0)
        {
            MenuPanelType previousType = panelHistory.Pop();
            
            // پنل فعلی رو ببند
            if(currentPanel != null) currentPanel.Close();
            
            // پنل قبلی رو پیدا کن و باز کن (بدون اضافه کردن دوباره به استک)
            MenuPanel prevPanel = allPanels.Find(p => p.panelType == previousType);
            if(prevPanel != null)
            {
                prevPanel.Open();
                currentPanel = prevPanel;
            }
        }
        else
        {
            // اگر هیچی تو استک نبود و تو منوی اصلی بودیم، خروج از بازی؟
            Debug.Log("No history left.");
        }
    }

    // تابع کمکی برای وصل کردن دکمه‌های ساده UI در اینسپکتور
    // این تابع رو به OnClick دکمه‌ها وصل کن و نوع پنل رو انتخاب کن
    public void OnNavigateButton(string panelTypeName)
    {
        // تبدیل string به Enum (برای اینکه بتونی تو اینسپکتور راحت باشی)
        if (System.Enum.TryParse(panelTypeName, out MenuPanelType type))
        {
            OpenPanel(type);
        }
    }

    // --- Action Handling ---

    public void OnStartGameClicked()
    {
        // منطق چک کردن قلب قبل از شروع
        if (GamePersistenceManager.Instance.TryConsumeHeart())
        {
            // صدا زدن لودینگ اسکرین یا مستقیم رفتن
            SceneManager.LoadScene("Game");
        }
        else
        {
            // باز کردن پنل خرید قلب
            OpenPanel(MenuPanelType.NotEnoughHeart);
        }
    }
    
    public void OnBuyHeartClicked()
    {
        // مثلا اتصال به تبلیغات یا کم کردن سکه
        if(GamePersistenceManager.Instance.SpendCoins(500))
        {
            GamePersistenceManager.Instance.data.currentHearts = 5;
            GamePersistenceManager.Instance.SaveGame();
            
            // رفرش UI و بستن پاپ آپ
            GoBack();
        }
    }

    // هندل کردن دکمه Back سخت‌افزاری اندروید
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPanel != null && currentPanel.panelType != MenuPanelType.MainMenu)
            {
                GoBack();
            }
            else
            {
                // خروج از بازی؟
                // Application.Quit();
            }
        }
    }
}
