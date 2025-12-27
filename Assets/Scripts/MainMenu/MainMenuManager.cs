using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [Header("Panels Reference")]
    public List<MenuPanel> allPanels;

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
        // اگر کاربر روی دکمه پنلی زد که همین الان بازه (به عنوان پنل فعال)، کاری نکن
        if (currentPanel != null && currentPanel.panelType == type) return;

        MenuPanel targetPanel = allPanels.Find(p => p.panelType == type);
        
        if (targetPanel != null)
        {
            if (currentPanel != null)
            {
                // --- شرط جدید: چه زمانی پنل قبلی بسته نشود؟ ---
                // اگر در منوی اصلی هستیم AND پنل جدید یکی از پنل‌های "روی هم" (Overlay) است
                // (به جای MenuPanelType.Tasks نام دقیق Enum خود را بنویسید)
                bool keepMainMenuOpen = (currentPanel.panelType == MenuPanelType.MainMenu) && 
                                        (type == MenuPanelType.Tasks /* || type == MenuPanelType.Settings */);

                if (!keepMainMenuOpen)
                {
                    currentPanel.Close();
                }

                // همیشه پنل قبلی را به تاریخچه اضافه کن تا دکمه Back کار کند
                panelHistory.Push(currentPanel.panelType);
            }

            targetPanel.Open();
            currentPanel = targetPanel;
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
            
            // پنل فعلی (مثلاً تسک‌ها) را ببند
            if(currentPanel != null) currentPanel.Close();
            
            // پنل قبلی (مثلاً منوی اصلی) را پیدا کن
            MenuPanel prevPanel = allPanels.Find(p => p.panelType == previousType);
            
            if(prevPanel != null)
            {
                // چک کن اگر پنل قبلی (منو) هنوز بازه (چون بسته نشده بود)، دیگه انیمیشن Open رو اجرا نکن
                if (!prevPanel.gameObject.activeInHierarchy)
                {
                    prevPanel.Open();
                }
                
                // پنل فعال رو برگردون به قبلی
                currentPanel = prevPanel;
            }
        }
        else
        {
            // اگر هیچی تو استک نبود و پنل فعلی منوی اصلی نبود، برگرد به منوی اصلی
            if (currentPanel != null && currentPanel.panelType != MenuPanelType.MainMenu)
            {
                OpenPanel(MenuPanelType.MainMenu);
            }
            else
            {
                // خروج از بازی در اندروید (اختیاری)
                // Application.Quit();
                Debug.Log("Already at Main Menu root.");
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

    // --- Action Handling ---
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
            // GamePersistenceManager.Instance.RestoreHearts(); // متد فرضی
            GoBack();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // اگر در منوی اصلی نیستیم، دکمه Back کار کنه
            if (currentPanel != null && currentPanel.panelType != MenuPanelType.MainMenu)
            {
                GoBack();
            }
        }
    }
}
