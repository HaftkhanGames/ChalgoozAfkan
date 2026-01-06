using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GamePersistenceManager : MonoBehaviour
{
    public static GamePersistenceManager Instance;

    [Header("Config")]
    public int maxHearts = 5;
    public int minutesPerHeart = 30; // هر 30 دقیقه یک قلب

    public PlayerData data;
    private string savePath;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        savePath = Application.persistentDataPath + "/playerdata.dat";
        LoadGame();
        GamePersistenceManager.Instance.AddCurrency(0, 0, 100, "Debug", "TestAdd");

        // بلافاصله بعد از لود، قلب‌ها را چک کن
        CalculateOfflineHearts();
    }

    // --- Save / Load Core ---
    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            try
            {
                data = (PlayerData)bf.Deserialize(file);
            }
            catch (Exception e)
            {
                Debug.LogError("Save Error: " + e.Message);
                data = new PlayerData();
                // ثبت خطا در آنالیتیکس
                AnalyticsManager.Instance.LogError("SaveFileCorrupted: " + e.Message);
            }
            finally { file.Close(); }
        }
        else
        {
            data = new PlayerData();
        }
    }

    // --- Heart / Energy System Logic ---

    private void CalculateOfflineHearts()
    {
        if (data.currentHearts >= maxHearts) return;

        DateTime lastTime = new DateTime(data.lastHeartRegenTime);
        DateTime now = DateTime.Now;
        TimeSpan timePassed = now - lastTime;
        
        int heartsToRecover = (int)timePassed.TotalMinutes / minutesPerHeart;

        if (heartsToRecover > 0)
        {
            int originalHearts = data.currentHearts;
            data.currentHearts += heartsToRecover;
            
            if (data.currentHearts >= maxHearts)
            {
                data.currentHearts = maxHearts;
                data.lastHeartRegenTime = DateTime.Now.Ticks;
            }
            else
            {
                data.lastHeartRegenTime += (long)(heartsToRecover * minutesPerHeart * TimeSpan.TicksPerMinute);
            }

            // [Analytics] ثبت قلب‌های بدست آمده به صورت آفلاین
            int gained = data.currentHearts - originalHearts;
            if(gained > 0)
            {
                AnalyticsManager.Instance.LogResourceGain("Hearts", gained, "System", "PassiveRegen");
            }
            
            SaveGame();
        }
    }
    
    public bool TryConsumeHeart()
    {
        CalculateOfflineHearts();

        if (data.currentHearts > 0)
        {
            if (data.currentHearts == maxHearts)
            {
                data.lastHeartRegenTime = DateTime.Now.Ticks;
            }

            data.currentHearts--;
            SaveGame();

            // [Analytics] ثبت مصرف قلب برای شروع بازی
            AnalyticsManager.Instance.LogResourceSpend("Hearts", 1, "Gameplay", "LevelStart");

            return true; 
        }
        PopupManager.Instance.ShowError("قلب ندارید!", 2f); 

        return false; 
    }
    
    public double GetSecondsToNextHeart()
    {
        if (data.currentHearts >= maxHearts) return 0;
        
        DateTime lastTime = new DateTime(data.lastHeartRegenTime);
        DateTime targetTime = lastTime.AddMinutes(minutesPerHeart);
        TimeSpan remaining = targetTime - DateTime.Now;
        
        return Math.Max(0, remaining.TotalSeconds);
    }

    // --- Decoration Logic ---

    public DecorationSaveData GetDecorationState(string decorationID)
    {
        if (data.decorations.ContainsKey(decorationID))
        {
            return data.decorations[decorationID];
        }
        return null; 
    }

    public void SetDecoration(string decorationID, int modelIndex)
    {
        // 1. اطمینان از لود بودن کل دیتا
        if (data == null)
        {
            data = new PlayerData(); // اگر دیتا کلاً نبود، بساز
        }

        // 2. فیکس ارور شما: اطمینان از ساخته شدن دیکشنری
        if (data.decorations == null)
        {
            data.decorations = new Dictionary<string, DecorationSaveData>();
        }

        // حالا این خط دیگر ارور نمی‌دهد
        bool isNew = !data.decorations.ContainsKey(decorationID);

        // ساخت آبجکت جدید با استفاده از Constructor که تعریف کردید
        DecorationSaveData decoData = new DecorationSaveData(true, modelIndex);

        if (isNew)
        {
            data.decorations.Add(decorationID, decoData);
        }
        else
        {
            data.decorations[decorationID] = decoData;
        }

        SaveGame();
        Debug.Log($"Decoration Saved: {decorationID} -> Model {modelIndex}");
    }


    // --- Currency Helpers (Modified for Analytics) ---
    
    /// <summary>
    /// اضافه کردن منابع با قابلیت ردیابی منبع درآمد
    /// </summary>
    /// <param name="sourceType">نوع منبع (مثلا Reward, Shop, DailyBonus)</param>
    /// <param name="sourceId">شناسه منبع (مثلا Level_01, Pack_Small)</param>
    public void AddCurrency(int coinsToAdd, int gemsToAdd, int starsToAdd, string sourceType = "Unknown", string sourceId = "Unknown")
    {
        if (coinsToAdd > 0)
        {
            data.coins += coinsToAdd;
            AnalyticsManager.Instance.LogResourceGain("Coins", coinsToAdd, sourceType, sourceId);
        }

        if (gemsToAdd > 0)
        {
            data.gems += gemsToAdd;
            AnalyticsManager.Instance.LogResourceGain("Gems", gemsToAdd, sourceType, sourceId);
        }

        if (starsToAdd > 0)
        {
            data.stars += starsToAdd;
            AnalyticsManager.Instance.LogResourceGain("Stars", starsToAdd, sourceType, sourceId);
        }

        SaveGame();
    }
    
    /// <summary>
    /// خرج کردن سکه با ردیابی آیتم خریداری شده
    /// </summary>
    public bool SpendCoins(int amount, string itemType = "Unknown", string itemId = "Unknown")
    {
        if(data.coins >= amount)
        {
            data.coins -= amount;
            SaveGame();
            
            // [Analytics]
            AnalyticsManager.Instance.LogResourceSpend("Coins", amount, itemType, itemId);
            
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// خرج کردن ستاره
    /// </summary>
    public bool SpendStars(int amount, string itemType = "Unknown", string itemId = "Unknown")
    {
        if(data.stars >= amount)
        {
            data.stars -= amount;
            SaveGame();

            // [Analytics]
            AnalyticsManager.Instance.LogResourceSpend("Stars", amount, itemType, itemId);

            return true;
        }
        return false;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }
    
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    // --- Reset Logic ---
    
    public void ResetAllData()
    {
        // 1. حذف فایل فیزیکی از حافظه دستگاه
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted successfully.");
        }

        // 2. ریست کردن دیتاهای درون حافظه (RAM)
        data = new PlayerData();
        
        // تنظیم مقادیر اولیه (مثلاً قلب پر)
        data.currentHearts = maxHearts;
        data.lastHeartRegenTime = DateTime.Now.Ticks;

        // اطمینان از اینکه دیکشنری دکوراسیون نال نباشد
        if (data.decorations == null)
            data.decorations = new Dictionary<string, DecorationSaveData>();

        // 3. ذخیره دیتای خالی جدید (برای اطمینان)
        SaveGame();

        // 4. لود مجدد صحنه برای به‌روزرسانی UI (اختیاری ولی پیشنهادی)
        // نیاز به using UnityEngine.SceneManagement; در بالای فایل دارید
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        Debug.Log("Game Data Reset Complete!");
    }

}
