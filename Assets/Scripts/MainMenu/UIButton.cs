using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UIElements;

public enum UIButtonActionType
{
    OpenPanel,
    LoadScene,
    OpenURL,
    QuitGame
}

[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [Header("Action Settings")]
    public UIButtonActionType actionType;

    [Tooltip("نام پنل یا صحنه یا URL")]
    public string targetValue;

    [Header("Menu Manager Reference")]
    public MenuManager menuManager; // فقط برای OpenPanel لازم میشه

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        // button.onClick.AddListener(ExecuteAction);
    }

    private void ExecuteAction()
    {
        switch (actionType)
        {
            case UIButtonActionType.OpenPanel:
                if (menuManager != null)
                {
                    menuManager.OpenPanel(targetValue);
                }
                break;

            case UIButtonActionType.LoadScene:
                SceneManager.LoadScene(targetValue);
                break;

            case UIButtonActionType.OpenURL:
                Application.OpenURL(targetValue);
                break;

            case UIButtonActionType.QuitGame:
                Application.Quit();
                break;
        }
    }
}