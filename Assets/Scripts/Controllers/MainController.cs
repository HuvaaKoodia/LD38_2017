using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController I;

    public NodeViewEvent onNodeSelected, onNodeDeselected;
    private NodeView selectedNode;

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            NodeView node;
            if (Helpers.ScreenPointToObject(out node, LayerMasks.Node))
            {
                DeselectNode();

                selectedNode = node;
                onNodeSelected(selectedNode);
            }
            else
                DeselectNode();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectNode();
        }
    }

    private void DeselectNode()
    {
        if (selectedNode != null)
        {
            onNodeDeselected(selectedNode);
        }
    }
}
