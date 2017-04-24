using UnityEngine;
using UnityEngine.UI;

public class DayLabel : MonoBehaviour
{
    public GameObject panel;
    public Text text;

	void Start ()
    {
        MainController.I.onDayEnd += OnDayEnd;

        panel.SetActive(false);
    }

    private void OnDayEnd()
    {
        panel.SetActive(true);
        text.text = "Day " + MainController.I.day; 
        AudioController.I.PlayAudio(AudioController.I.nextDaySource);
    }

    public void OnPressed()
    {
        MainController.I.DayStart();
        panel.SetActive(false);
    }
}
