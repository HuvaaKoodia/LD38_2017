using UnityEngine;
using UnityEngine.UI;

public class DiscussionButton : MonoBehaviour
{
    public CharacterEvent onButtonPressedEvent;
    public CharacterView character { get; set; }

    void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(OnButtonPressed);
    }

    public void OnButtonPressed()
    {
        onButtonPressedEvent(character);
    }
}
