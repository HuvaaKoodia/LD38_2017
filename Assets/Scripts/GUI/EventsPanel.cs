using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsPanel : MonoBehaviour
{
    public GameObject panel;
    public EventLabel eventPrefab;
    public Transform eventsParent;

    public void ShowEventsHack(bool show)
    {
        panel.SetActive(show);
    }

    private void Start()
    {
        MainController.I.onKnownEventAdded += EventAdded;
        MainController.I.onKnownEventRemoved += EventRemoved;

        panel.SetActive(false);
    }

    private void EventAdded(Event e)
    {
        panel.SetActive(true);

        var eventLabel = Instantiate(eventPrefab, eventsParent);
        eventLabel.gameObject.SetActive(true);
        eventLabel.SetEvent(e);

        LayoutRebuilder.ForceRebuildLayoutImmediate(eventsParent.parent as RectTransform);
    }

    private void EventRemoved(Event e)
    {
        for (int i = 0; i < eventsParent.childCount; i++)
        {
            var label = eventsParent.GetChild(i);
            if (label.GetComponent<EventLabel>().e == e)
            {
                DestroyImmediate(label.gameObject);
                break;
            }
        }

        if (eventsParent.childCount == 0)
            panel.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(eventsParent.parent as RectTransform);
    }
}
