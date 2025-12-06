using UnityEngine;
using System.Collections;

public class FaceExpressionSystem : MonoBehaviour
{
    public enum ExpressionType
    {
        None,
        Happy,
        Attack,
        Dead
    }

    [Header("Expression Objects")]
    public GameObject happyObj;
    public GameObject attackObj;
    public GameObject deadObj;

    private Coroutine currentRoutine;

    public void ShowExpression(ExpressionType type, float duration)
    {
        // اگر قبلی هنوز در حال اجراست، قطعش کن
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayExpression(type, duration));
    }

    private IEnumerator PlayExpression(ExpressionType type, float duration)
    {
        DisableAll();

        GameObject target = GetObject(type);
        if (target != null)
            target.SetActive(true);

        yield return new WaitForSeconds(duration);

        DisableAll();
        currentRoutine = null;
    }

    private GameObject GetObject(ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Happy: return happyObj;
            case ExpressionType.Attack: return attackObj;
            case ExpressionType.Dead: return deadObj;
        }
        return null;
    }

    private void DisableAll()
    {
        happyObj?.SetActive(false);
        attackObj?.SetActive(false);
        deadObj?.SetActive(false);
    }
}