using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Linq; // برای استفاده از توابع راحت لیست

public class EnvironmentLoader : MonoBehaviour
{
    public static EnvironmentLoader Instance;

    public AssetReference environmentPrefabRef; 
    private GameObject loadedEnvironment;
    private List<DecorationSpotController> allSpots = new List<DecorationSpotController>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadEnvironment(); // یا هر جا که صلاح میدونید
    }

    private void LoadEnvironment()
    {
        environmentPrefabRef.InstantiateAsync().Completed += OnEnvironmentLoaded;
    }

    private void OnEnvironmentLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedEnvironment = handle.Result;
            
            // گرفتن تمام نقاط
            var spots = loadedEnvironment.GetComponentsInChildren<DecorationSpotController>(true);
            allSpots.AddRange(spots);

            // 1. مقداردهی اولیه (لود کردن سیو ها)
            foreach (var spot in allSpots)
            {
                spot.Initialize();
            }

            // 2. <--- محاسبه اینکه کدام مرحله‌ها باید باز باشند
            RefreshProgression();

            Debug.Log("Environment Loaded and Initialized.");
        }
        else
        {
            Debug.LogError("Failed to load environment.");
        }
    }

    // <--- متد جدید و مهم برای مدیریت توالی مراحل
    // در کلاس EnvironmentLoader جایگزین متد قبلی کنید
    public void RefreshProgression()
    {
        if (allSpots == null || allSpots.Count == 0) return;

        int maxStep = allSpots.Max(s => s.sequenceStep);
        bool previousStepComplete = true;
    
        // لیستی برای ذخیره تسک‌های مرحله جاری
        List<DecorationSpotController> currentStageTasks = new List<DecorationSpotController>();
        bool foundCurrentStage = false;

        for (int step = 0; step <= maxStep; step++)
        {
            var spotsInThisStep = allSpots.Where(s => s.sequenceStep == step).ToList();
            bool isThisStepComplete = spotsInThisStep.All(s => s.IsBuilt);

            foreach (var spot in spotsInThisStep)
            {
                spot.SetInteractable(previousStepComplete);
            }

            // پیدا کردن تسک‌های فعال برای نمایش در UI
            // اولین مرحله‌ای که کامل نشده (isThisStepComplete == false) و باز است (previousStepComplete == true)
            // مرحله جاری محسوب می‌شود.
            if (previousStepComplete && !isThisStepComplete && !foundCurrentStage)
            {
                currentStageTasks = spotsInThisStep; // کل اسپات‌های این مرحله
                foundCurrentStage = true; // مرحله جاری پیدا شد، مراحل بعدی دیگر تسک فعال نیستند
            }

            previousStepComplete = isThisStepComplete;
        }

        // ارسال لیست به UI Manager
        if (TaskUIManager.Instance != null)
        {
            // ما لیست کل اسپات‌های مرحله جاری را می‌فرستیم
            // خودِ TaskUIManager چک می‌کند کدام‌ها IsBuilt نیستند و نمایش می‌دهد
            TaskUIManager.Instance.UpdateTasks(currentStageTasks);
        }
    }

}
