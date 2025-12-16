using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Configuration")]
    [SerializeField] private LevelData[] allLevels; 
    [SerializeField] private Transform levelSpawnPoint; 

    private GameObject currentLevelInstance;
    private LevelData currentLevelData;
    private int currentLevelIndex; // ایندکس واقعی (از 0 شروع می‌شود)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // دریافت مرحله انتخاب شده
        currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        // ثبت شروع مرحله در آنالیتیکس
        AnalyticsManager.Instance.LogLevelStart(currentLevelIndex + 1);

        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= allLevels.Length) return;

        if (currentLevelInstance != null) Destroy(currentLevelInstance);

        currentLevelData = allLevels[index];
        
        // ساخت مرحله
        Vector3 spawnPos = levelSpawnPoint != null ? levelSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = levelSpawnPoint != null ? levelSpawnPoint.rotation : Quaternion.identity;

        currentLevelInstance = Instantiate(currentLevelData.levelPrefab, spawnPos, spawnRot);
        currentLevelInstance.transform.SetParent(this.transform); 
    }

    /// <summary>
    /// این تابع زمانی صدا زده می‌شود که بازیکن برنده شود
    /// مقادیر سکه و ستاره از گیم‌پلی پاس داده می‌شوند
    /// </summary>
    /// <param name="collectedCoins">تعداد سکه‌ای که در این دور جمع کرده</param>
    /// <param name="earnedStars">تعداد ستاره‌ای که در این دور گرفته (1 تا 3)</param>
    public void LevelWon(int collectedCoins, int earnedStars)
    {
        // 1. ثبت پایان مرحله در آنالیتیکس (با امتیاز کلی این دور)
        AnalyticsManager.Instance.LogLevelComplete(currentLevelIndex + 1, earnedStars);

        // --- محاسبه منطق ستاره‌ها (One-time Reward Logic) ---
        int previousBestStars = 0;
        var scores = GamePersistenceManager.Instance.data.levelScores;

        // چک می‌کنیم قبلاً در این مرحله چند ستاره گرفته بوده
        if (scores.ContainsKey(currentLevelIndex))
        {
            previousBestStars = scores[currentLevelIndex];
        }

        // محاسبه ستاره جدیدی که باید به کارنسی اضافه شود
        // مثال: قبلا 2 بوده، الان 3 شده -> 2 - 3 = 1 ستاره جایزه می‌گیرد
        // مثال: قبلا 3 بوده، الان 2 شده -> شرط برقرار نیست، 0 ستاره می‌گیرد
        int starsCurrencyToAdd = 0;
        if (earnedStars > previousBestStars)
        {
            starsCurrencyToAdd = earnedStars - previousBestStars;
            
            // آپدیت کردن رکورد جدید در دیتا
            if (scores.ContainsKey(currentLevelIndex))
                scores[currentLevelIndex] = earnedStars;
            else
                scores.Add(currentLevelIndex, earnedStars);
        }

        // --- محاسبه منطق سکه‌ها (Always Add Logic) ---
        // سکه همیشه اضافه می‌شود، چه بار اول باشد چه بار صدم
        int coinsToAdd = collectedCoins;

        // --- ذخیره و اعمال تغییرات ---
        
        // 1. اضافه کردن کارنسی‌ها
        GamePersistenceManager.Instance.AddCurrency(
            coinsToAdd, 
            0, // Gems
            starsCurrencyToAdd, // فقط مابه‌التفاوت ستاره‌ها اضافه می‌شود
            "LevelComplete", 
            $"Level_{currentLevelIndex + 1}"
        );

        // 2. باز کردن مرحله بعد (Unlock Next Level)
        int nextLevelIndex = currentLevelIndex + 1;
        if (nextLevelIndex > GamePersistenceManager.Instance.data.highestUnlockedLevelIndex)
        {
            GamePersistenceManager.Instance.data.highestUnlockedLevelIndex = nextLevelIndex;
        }

        // 3. ذخیره فایل
        GamePersistenceManager.Instance.SaveGame();

        Debug.Log($"Level Won! Coins Added: {coinsToAdd}, New Stars Added: {starsCurrencyToAdd}");
        
        // اینجا می‌توانید UI برد را صدا بزنید و مقادیر را نمایش دهید
        // UIManager.Instance.ShowWinPanel(coinsToAdd, starsCurrencyToAdd);
    }

    public void LevelLost()
    {
        AnalyticsManager.Instance.LogLevelFail(currentLevelIndex + 1);
        Debug.Log("Level Fail!");
        // UIManager.Instance.ShowLosePanel();
    }

    public void OnRetryLevel()
    {
        if (GamePersistenceManager.Instance.TryConsumeHeart())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log("Not enough hearts!");
            // PopupManager.Instance.ShowNoHearts();
        }
    }

    public void OnGoHome()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
