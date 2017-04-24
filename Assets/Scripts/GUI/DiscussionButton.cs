using UnityEngine;
using UnityEngine.UI;

public delegate void DiscussionButtonEvent(DiscussionButton button);
public class DiscussionButton : MonoBehaviour
{
    public DiscussionButtonEvent onButtonPressedEvent;
    public CharacterView character { get; set; }
    public int Index;

    void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(OnButtonPressed);
    }

    public void OnButtonPressed()
    {
        onButtonPressedEvent(this);
    }
}
