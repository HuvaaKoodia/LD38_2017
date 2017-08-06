using System;
using UnityEngine;
using UnityEngine.UI;

public class DaysLeftText : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        gameObject.SetActive(false);
        MainController.I.onFirstDayStart += Show;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    void Update() //lazy polling
    {
        text.text = string.Format("Day {0}, {1} {2}. Days left: {3}", MainController.I.day, MainController.GetDayName(MainController.I.day),MainController.I.actionPoints == MainController.I.actionPointsPerDay ? "Morning" : "Evening", MainController.I.daysLeft);//super lazy!
    }
}
