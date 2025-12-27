using UnityEngine;
using System.Collections.Generic;

public class TaskUIManager : MonoBehaviour
{
    public static TaskUIManager Instance;

    [Header("References")]
    public Transform taskContainer;     // جایی که آیتم‌ها باید ساخته شوند (Content داخل ScrollView)
    public GameObject taskItemPrefab;   // پریفب TaskItemUI که در مرحله قبل ساختید
    public GameObject emptyStateObject; // (اختیاری) متنی که وقتی تسکی نیست نمایش داده شود ("همه کارها انجام شد")

    private void Awake()
    {
        Instance = this;
    }

    // این متد توسط EnvironmentLoader صدا زده می‌شود
    public void UpdateTasks(List<DecorationSpotController> activeSpots)
    {
        // 1. پاک کردن لیست قبلی
        foreach (Transform child in taskContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. اگر تسکی نیست
        if (activeSpots == null || activeSpots.Count == 0)
        {
            if (emptyStateObject) emptyStateObject.SetActive(true);
            return;
        }

        if (emptyStateObject) emptyStateObject.SetActive(false);

        // 3. ساختن آیتم‌های جدید
        foreach (var spot in activeSpots)
        {
            // فقط آیتم‌هایی که هنوز ساخته نشده‌اند را در لیست تسک نشان بده
            if (!spot.IsBuilt)
            {
                GameObject newItem = Instantiate(taskItemPrefab, taskContainer);
                TaskItemUI uiScript = newItem.GetComponent<TaskItemUI>();
                if (uiScript != null)
                {
                    uiScript.Setup(spot);
                }
            }
        }
    }
}