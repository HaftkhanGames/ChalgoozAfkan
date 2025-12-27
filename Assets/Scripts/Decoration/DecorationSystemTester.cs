using UnityEngine;
using System.Collections;

public class DecorationSystemTester : MonoBehaviour
{
    [Header("Refrences")]
    public EnvironmentLoader environmentLoader; // دستی در اینسپکتور بدهید
    
    [Header("Debug Info")]
    public DecorationSpotController targetSpot; // اولین اسپاتی که پیدا کنه اینجا میذاره

    private void Start()
    {
        Debug.Log("TEST START: Waiting for Environment to Load...");
        StartCoroutine(WaitForEnvironmentAndSetup());
    }

    private IEnumerator WaitForEnvironmentAndSetup()
    {
        // 1. صبر میکنیم تا EnvironmentLoader کارش تموم شه
        // (فرض بر اینه که EnvironmentLoader یک متغیر برای چک کردن وضعیت داره یا ما چک میکنیم اسپات ها اومدن یا نه)
        yield return new WaitForSeconds(1.0f); 

        // 2. پیدا کردن اولین نقطه دکوراسیون در صحنه برای تست
        targetSpot = FindObjectOfType<DecorationSpotController>();

        if (targetSpot != null)
        {
            Debug.Log($"<color=green>Target Found:</color> {targetSpot.name} | ID: {targetSpot.itemData.id}");
            Debug.Log("Controls: Press '1', '2', '3' to Preview. Press 'Space' to Save/Buy. Press 'R' to Reload Logic.");
        }
        else
        {
            Debug.LogError("No DecorationSpotController found! Make sure the Environment Prefab is loaded.");
        }
    }

    private void Update()
    {
        if (targetSpot == null) return;

        // --- شبیه سازی کلیک روی دکمه های UI ---

        // کلید 1: پیش نمایش مدل اول
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("UI: User clicked Variant 0");
            targetSpot.PreviewModel(0);
        }

        // کلید 2: پیش نمایش مدل دوم
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("UI: User clicked Variant 1");
            targetSpot.PreviewModel(1);
        }

        // کلید 3: پیش نمایش مدل سوم
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("UI: User clicked Variant 2");
            targetSpot.PreviewModel(2);
        }

        // --- شبیه سازی دکمه خرید/تایید ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("<color=yellow>UI: User clicked Confirm/Buy</color>");
            
            // اینجا فرض میکنیم پول کاربر کافی بوده و پرداخت انجام شده
            // targetSpot.ConfirmAndSave();
            
            Debug.Log($"<color=green>SAVED!</color> Check Console/SaveFile for ID: {targetSpot.itemData.id}");
        }

        // --- تست بارگذاری مجدد (Re-Initialize) ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Simulating Scene Reload / Re-entry...");
            // این کار باعث میشه اسپات دوباره بره از سیو بخونه
            // اگر سیو درست کار کرده باشه، باید مدلی که اسپیس زدید لود بشه
            targetSpot.Initialize(); 
        }
    }
}
