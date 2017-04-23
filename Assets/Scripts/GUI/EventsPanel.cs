using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventsPanel : MonoBehaviour
{
    public GameObject panel;
    public EventLabel eventPrefab;
    public Transform eventsParent;

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
        int destroyed = 0;
        foreach (Transform label in eventsParent)
        {
            if (label.GetComponent<EventLabel>().e == e)
            {
                Destroy(label.gameObject);
                destroyed++;
            }
        }

        if (eventsParent.childCount - destroyed == 0)
            panel.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(eventsParent.parent as RectTransform);
    }
}
