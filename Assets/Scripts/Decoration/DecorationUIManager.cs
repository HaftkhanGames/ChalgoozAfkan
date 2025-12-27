using UnityEngine;

public class DecorationUIManager : MonoBehaviour
{
    private DecorationSpotController currentSpot;
    private int selectedVariantIndex;

    // این متد رو وقتی پلیر روی آبجکت توی صحنه کلیک کرد صدا بزن
    public void OpenMenu(DecorationSpotController spot)
    {
        currentSpot = spot;
        // اینجا پنل UI رو باز میکنی و دکمه ها رو بر اساس itemData میسازی
        // ...
    }

    // متصل به دکمه مدل 1، 2، 3
    public void OnVariantSelected(int index)
    {
        selectedVariantIndex = index;
        
        // فقط نمایش بده (هنوز سیو نشده)
        currentSpot.PreviewModel(index);
    }

    // متصل به دکمه "خرید/تایید"
    public void OnConfirmClicked()
    {
        // 1. چک کردن پول (با استفاده از داده های SO)
        var variantData = currentSpot.itemData.variants[selectedVariantIndex];
        
        bool canAfford = false;
        if(variantData.price.currencyType == CurrencyType.Coin)
            canAfford = GamePersistenceManager.Instance.SpendCoins(variantData.price.amount, "Deco", currentSpot.itemData.id);
        // ... بقیه ارزها

        if (canAfford || variantData.price.IsFree)
        {
            // 2. تایید نهایی و سیو
            // currentSpot.ConfirmAndSave();
            
            // بستن منو
            // CloseMenu();
        }
        else
        {
            Debug.Log("پول نداری!");
        }
    }
}