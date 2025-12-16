using UnityEngine;
using UnityEngine.Events;

public class MenuPanel : MonoBehaviour
{
    public MenuPanelType panelType;
    
    // ایونت‌هایی که موقع باز و بسته شدن پنل اجرا میشن (برای انیمیشن یا صدا)
    public UnityEvent onOpen;
    public UnityEvent onClose;

    public void Open()
    {
        gameObject.SetActive(true);
        onOpen?.Invoke();
    }

    public void Close()
    {
        onClose?.Invoke();
        gameObject.SetActive(false);
    }
}