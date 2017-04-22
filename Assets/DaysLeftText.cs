using UnityEngine;
using UnityEngine.UI;

public class DaysLeftText : MonoBehaviour
{
    public Text text;

    void Update()
    {
        text.text = string.Format("Day {0}, {1}. Days left: {2}", MainController.I.day, MainController.GetDayName(MainController.I.day), MainController.I.daysLeft);//super lazy!
    }
}
