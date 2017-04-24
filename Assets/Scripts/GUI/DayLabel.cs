using UnityEngine;
using UnityEngine.UI;

public class DayLabel : MonoBehaviour
{
    public Text text;

	void Start ()
    {
        MainController.I.onDayEnd += OnDayEnd;

        text.gameObject.SetActive(false);
    }

    private void OnDayEnd()
    {
        text.gameObject.SetActive(true);
        text.text = "Day " + MainController.I.day;
        AudioController.I.PlayAudio(AudioController.I.nextDaySource);
    }

    public void OnPressed()
    {
        MainController.I.DayStart();
        text.gameObject.SetActive(false);
    }
}
