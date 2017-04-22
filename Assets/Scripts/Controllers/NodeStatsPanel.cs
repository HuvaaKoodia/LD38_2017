using System;
using UnityEngine;

public class NodeStatsPanel : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        MainController.I.onNodeSelected += OpenPanel;
        MainController.I.onNodeDeselected += ClosePanel;

        panel.SetActive(false);
    }

    private void OpenPanel(NodeView node)
    {
        panel.SetActive(true);
    }

    private void ClosePanel(NodeView node)
    {
        panel.SetActive(false);
    }

}
