using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [Header("Registered Panels")]
    public List<UIPanel> panels = new List<UIPanel>();

    private UIPanel currentPanel;

    public void OpenPanel(string id)
    {
        UIPanel target = panels.Find(x => x.panelID == id);

        if (target == null)
        {
            Debug.LogWarning($"Panel '{id}' not found!");
            return;
        }

        if (currentPanel != null)
            currentPanel.Hide();

        target.Show();
        currentPanel = target;
    }
}