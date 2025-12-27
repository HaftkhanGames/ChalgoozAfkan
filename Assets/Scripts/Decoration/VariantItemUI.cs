using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class VariantItemUI : MonoBehaviour
{
    [Header("References")]
    public Image thumbnailImage;       // عکس آیتم
    public GameObject selectionBorder; // بردر یا افکت انتخاب شده
    public Button clickButton;         // دکمه اصلی

    private int myIndex;
    private UnityAction<int> onSelected;

    // این متد را پنل صدا می‌زند تا اطلاعات را در دکمه بریزد
    public void Initialize(int index, Sprite icon, bool isSelected, UnityAction<int> callback)
    {
        myIndex = index;
        onSelected = callback;

        // تنظیم عکس از روی دیتای شما
        if(thumbnailImage != null)
            thumbnailImage.sprite = icon;

        // تنظیم وضعیت انتخاب
        SetSelectionState(isSelected);

        // تنظیم کلیک
        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(() => onSelected.Invoke(myIndex));
    }

    public void SetSelectionState(bool isSelected)
    {
        if (selectionBorder != null)
            selectionBorder.SetActive(isSelected);
    }
}