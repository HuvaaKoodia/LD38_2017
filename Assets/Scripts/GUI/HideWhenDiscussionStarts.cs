﻿using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HideWhenDiscussionStarts : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public bool showWhenEventChoice = false;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        DiscussionController.I.onDiscussionStart += Hide;
        DiscussionController.I.onDiscussionEnd += Show;

        if (showWhenEventChoice)
        {
            DiscussionController.I.onEventChoiceButtons += Show;
            DiscussionController.I.onEventChosen += Hide;
        }
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
    }
}
