using UnityEngine;
using UnityEngine.UI;

public class DaysLeftText : MonoBehaviour
{
    public Text text;

    void Update()
    {
        text.text = string.Format("Day {0}, {1} {2}. Days left: {3}", MainController.I.day, MainController.GetDayName(MainController.I.day),MainController.I.actionPoints == MainController.I.actionPointsPerDay ? "Morning" : "Evening", MainController.I.daysLeft);//super lazy!
    }
}
