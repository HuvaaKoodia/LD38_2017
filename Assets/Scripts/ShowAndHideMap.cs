
using UnityEngine;

public class ShowAndHideMap : MonoBehaviour
{
    void Start()
    {
        DiscussionController.I.onDiscussionStart += Hide;
        DiscussionController.I.onDiscussionEnd += Show;
        MainController.I.onGameOver += Hide;
    }

    private void Hide()
    {
        CharacterView.ShowAllLines(false);

        foreach (var ch in MainController.I.otherCharacters)
        {
            if (ch == DiscussionController.I.playerCharacter || ch == DiscussionController.I.otherCharacter) continue;

            ch.gameObject.SetActive(false);
        }
    }

    private void Show()
    {
        CharacterView.ShowAllLines(true);

        foreach (var ch in MainController.I.otherCharacters)
        {

            ch.gameObject.SetActive(true);
        }
    }
}
