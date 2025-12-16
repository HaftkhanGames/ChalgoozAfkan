using System.Collections.Generic;
using System; // برای استفاده از DateTime

[System.Serializable]
public class PlayerData
{
    // --- Currencies ---
    public int coins;
    public int gems;
    public int stars; // واحد خرج کردنی برای دکوراسیون

    // --- Energy (Heart) System ---
    public int currentHearts;
    public long lastHeartRegenTime; // ذخیره زمان به صورت Ticks (عدد طولانی)

    // --- Progression ---
    public int highestUnlockedLevelIndex;
    // کلید: ایندکس مرحله، مقدار: امتیاز ستاره‌ای که در بازی گرفته (1 تا 3)
    public Dictionary<int, int> levelScores; 

    // --- Decorations ---
    // کلید: شناسه دکوراسیون (مثلاً "LivingRoom_Sofa")
    // مقدار: اطلاعات ذخیره شده آن دکور
    public Dictionary<string, DecorationSaveData> decorations;

    // --- Constructor ---
    public PlayerData()
    {
        coins = 0;
        gems = 0;
        stars = 0;
        
        currentHearts = 5; // مقدار اولیه قلب (مثلاً پر است)
        lastHeartRegenTime = DateTime.Now.Ticks;

        highestUnlockedLevelIndex = 0;
        levelScores = new Dictionary<int, int>();
        decorations = new Dictionary<string, DecorationSaveData>();
    }
}

// کلاس کمکی برای ذخیره وضعیت هر دکوراسیون
[System.Serializable]
public class DecorationSaveData
{
    public bool isBuilt;
    public int selectedModelIndex; // 0 پیش‌فرض، 1 و 2 مدل‌های دیگر

    public DecorationSaveData(bool built, int modelIndex)
    {
        isBuilt = built;
        selectedModelIndex = modelIndex;
    }
}