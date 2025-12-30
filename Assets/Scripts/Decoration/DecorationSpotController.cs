using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class DecorationSpotController : MonoBehaviour
{
    [Header("Config")]
    public DecorationItemSO itemData;

    [Header("Progression")]
    [Tooltip("شماره مرحله‌ای که این آیتم در آن فعال می‌شود. 0 برای شروع.")]
    public int sequenceStep = 0;

    [Header("Visuals")]
    public List<GameObject> modelObjects; // لیست مدل‌های مختلف (Variant) باید دقیقا هم‌تعداد variants در SO باشد
    public GameObject interactionIcon;    // آیکون چکش

    // وضعیت داخلی
    private int currentActiveIndex = -1;
    private Collider col; 

    public bool IsBuilt => currentActiveIndex != -1;

    private void Awake()
    {
        col = GetComponent<Collider>();
        
        // اطمینان از خاموش بودن مدل‌ها در لحظه شروع
        foreach (var model in modelObjects)
        {
            if (model != null) model.SetActive(false);
        }
        
        if (interactionIcon != null) interactionIcon.SetActive(false);
    }

    public void Initialize()
    {
        var savedData = GamePersistenceManager.Instance.GetDecorationState(itemData.id);

        if (savedData != null && savedData.isBuilt)
        {
            currentActiveIndex = savedData.selectedModelIndex;
            ActivateModel(currentActiveIndex, false);
            SetInteractable(false); // اگر ساخته شده، تعامل قفل شود
        }
    }

    // ------------------- تغییرات جدید برای سیستم انتخاب -------------------

    /// <summary>
    /// این متد فقط مدل را برای نمایش عوض می‌کند.
    /// هیچ چیزی ذخیره نمی‌شود و مرحله جلو نمی‌رود.
    /// توسط پنل انتخاب (Selection Panel) صدا زده می‌شود.
    /// </summary>
    public void PreviewModel(int index)
    {
        ActivateModel(index, true);
    }

    /// <summary>
    /// این متد نهایی است. وقتی دکمه CONFIRM در پنل زده شد اجرا می‌شود.
    /// </summary>
    public void ConfirmAndFinish(int finalIndex)
    {
        // 1. ذخیره در دیتابیس
        GamePersistenceManager.Instance.SetDecoration(itemData.id, finalIndex);

        // 2. ست کردن ایندکس نهایی (اگر احیانا ست نشده باشد)
        currentActiveIndex = finalIndex;

        // 3. قفل کردن تعامل (چون ساخت تمام شده)
        SetInteractable(false);

        // 4. خبر دادن به سیستم برای باز کردن مراحل بعدی
        EnvironmentLoader.Instance.RefreshProgression();

        // 5. (اختیاری) پخش پارتیکل یا صدای موفقیت
        Debug.Log($"[DecorationSpot] {itemData.itemName} Confirmed with variant {finalIndex}.");
    }

    // ----------------------------------------------------------------------

    public void SetInteractable(bool canInteract)
    {
        // اگر قبلاً ساخته شده، دیگر قابل تعامل نیست (مگر اینکه سیستم ادیت داشته باشید)
        if (IsBuilt)
        {
            if (interactionIcon) interactionIcon.SetActive(false);
            if (col) col.enabled = false; 
            return;
        }

        // اگر نوبت این دکور است
        if (interactionIcon) interactionIcon.SetActive(canInteract);
        if (col) col.enabled = canInteract;
    }

    // در DecorationSpotController.cs

    private void ActivateModel(int index, bool animate)
    {
        if (index < 0 || index >= modelObjects.Count) return;

        // خاموش کردن همه
        for (int i = 0; i < modelObjects.Count; i++)
        {
            if (i != index) modelObjects[i].SetActive(false);
        }

        GameObject targetModel = modelObjects[index];
        
        // نکته مهم: قبل از انیمیشن حتما SetActive(true) شود
        targetModel.SetActive(true);
        currentActiveIndex = index;

        if (animate)
        {
            // از اسکیل 0 شروع میکنیم به 1
            targetModel.transform.localScale = Vector3.zero;
            targetModel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            targetModel.transform.localScale = Vector3.one;
        }
    }

}
