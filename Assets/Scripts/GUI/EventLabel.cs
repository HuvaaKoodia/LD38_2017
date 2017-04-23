using UnityEngine;
using UnityEngine.UI;

public class EventLabel : MonoBehaviour
{
    public Text text;
    public Event e { get; private set; }

    public void SetEvent(Event e) {
        this.e = e;

        var temp = "";
        for (int i = 0; i < e.participants.Length; i++)
        {
            temp += e.participants[i].data.name;
            if (i < e.participants.Length - 1) temp += ", ";
        }

        text.text = string.Format("{0}\nDay {1}, {2}\n{3}", e.Name ,e.day, MainController.GetDayName(e.day), temp);

    }
}
