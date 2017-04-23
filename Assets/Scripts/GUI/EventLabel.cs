using UnityEngine;
using UnityEngine.UI;

public class EventLabel : MonoBehaviour
{
    public Text text;
    public Event e { get; private set; }

    public void SetEvent(Event e) {
        this.e = e;

        var participantsList = e.GetParticipantsList(false);

        text.text = string.Format("{0}\nDay {1}, {2}\n{3}", e.Name ,e.day, MainController.GetDayName(e.day), participantsList);

    }
}
