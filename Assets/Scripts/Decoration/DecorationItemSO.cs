using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewDecorationData", menuName = "Decoration/Item Data")]
public class DecorationItemSO : ScriptableObject
{
    public string id; // شناسه منحصر به فرد (مثلا Garden_Tree)
    public string decorationName;
    public string itemName; // شناسه منحصر به فرد (مثلا Garden_Tree)

    [Header("UI Info")]
    public Sprite icon; // آیکون اصلی

    public int star;
    [Header("Variants Info")]
    // اطلاعات مربوط به هر مدل (قیمت، تامنیل و ...)
    public List<DecorationVariantData> variants;
}

[System.Serializable]
public class DecorationVariantData
{
    public Sprite thumbnail; // عکس کوچک برای دکمه انتخاب
    public Price price;      // قیمت این مدل خاص (میتونه رایگان باشه)
}