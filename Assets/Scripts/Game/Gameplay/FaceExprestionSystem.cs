using UnityEngine;
using System.Collections;

public class FaceExpressionSystem : MonoBehaviour
{
    public enum ExpressionType
    {
        Neutral, // حالت پیش‌فرض
        Happy,
        Attack,
        Hurt,    // اضافه شده برای دمیج
        Dead
    }

    [Header("Expression Objects")]
    [Tooltip("آبجکت چهره عادی که همیشه فعال است مگر اینکه حالت دیگری جایگزین شود")]
    public GameObject neutralObj;
    public GameObject happyObj;
    public GameObject attackObj;
    public GameObject hurtObj;
    public GameObject deadObj;

    private Coroutine currentRoutine;
    private bool isDead = false; // قفل کردن سیستم در صورت مرگ

    private void Start()
    {
        // شروع بازی با حالت عادی
        SetExpression(ExpressionType.Neutral);
    }

    /// <summary>
    /// نمایش یک حالت چهره برای مدت مشخص و سپس بازگشت به حالت عادی
    /// </summary>
    public void ShowExpressionTemporary(ExpressionType type, float duration)
    {
        if (isDead) return; // اگر مرده، چهره عوض نشود

        // اگر قبلی هنوز در حال اجراست، قطعش کن
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayExpressionRoutine(type, duration));
    }

    /// <summary>
    /// نمایش یک حالت چهره به صورت دائمی (مثل مرگ)
    /// </summary>
    public void SetPermanentExpression(ExpressionType type)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        
        SetExpression(type);

        if (type == ExpressionType.Dead)
            isDead = true;
    }

    // متد کمکی برای اتصال راحت به UnityEvents (چون Enum را مستقیم نمی‌توان در ایونت‌های ساده فرستاد)
    public void ShowHurt(float duration) => ShowExpressionTemporary(ExpressionType.Hurt, duration);
    public void ShowHappy(float duration) => ShowExpressionTemporary(ExpressionType.Happy, duration);
    public void ShowAttack(float duration) => ShowExpressionTemporary(ExpressionType.Attack, duration);
    public void SetDead() => SetPermanentExpression(ExpressionType.Dead);


    private IEnumerator PlayExpressionRoutine(ExpressionType type, float duration)
    {
        SetExpression(type);

        yield return new WaitForSeconds(duration);

        // بازگشت به حالت عادی
        SetExpression(ExpressionType.Neutral);
        currentRoutine = null;
    }

    private void SetExpression(ExpressionType type)
    {
        DisableAll();

        GameObject target = GetObject(type);
        if (target != null)
            target.SetActive(true);
    }

    private GameObject GetObject(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Neutral: return neutralObj;
            case ExpressionType.Happy: return happyObj;
            case ExpressionType.Attack: return attackObj;
            case ExpressionType.Hurt: return hurtObj;
            case ExpressionType.Dead: return deadObj;
        }
        return null;
    }

    private void DisableAll()
    {
        if(neutralObj) neutralObj.SetActive(false);
        if(happyObj) happyObj.SetActive(false);
        if(attackObj) attackObj.SetActive(false);
        if(hurtObj) hurtObj.SetActive(false);
        if(deadObj) deadObj.SetActive(false);
    }
}
