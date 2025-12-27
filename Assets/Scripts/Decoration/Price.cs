using System;
using UnityEngine;

// نوع ارز پرداختی
public enum CurrencyType
{
    Free,
    Coin,
    Gem
}

// ساختار قیمت برای نمایش در اینسپکتور
[Serializable]
public struct Price
{
    public CurrencyType currencyType;
    public int amount;

    // متد کمکی برای چک کردن رایگان بودن
    public bool IsFree => currencyType == CurrencyType.Free || amount <= 0;
}