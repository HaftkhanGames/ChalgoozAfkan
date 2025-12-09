using UnityEngine;
using DG.Tweening;

public class UIPanel : MonoBehaviour
{
    public string panelID;
    public CanvasGroup canvasGroup;

    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.3f);
    }

    public void Hide()
    {
        canvasGroup.DOFade(0, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }
}