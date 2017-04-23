using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class HideWhenDiscussionStarts : MonoBehaviour
{
    CanvasGroup canvasGroup;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        DiscussionController.I.onDiscussionStart += Hide;
        DiscussionController.I.onDiscussionEnd += Show;
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
