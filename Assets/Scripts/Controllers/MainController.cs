using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void EventEvent(Event e);//LAL!

public enum EventType
{
    party,
    meeting
}

[Serializable]
public class Event
{
    public string Name;
    public int day;
    public EventType eventType { get; set; }

    public CharacterView[] participants;
}

public class MainController : MonoBehaviour
{
    public static MainController I;

    public CharacterView playerCharacter;
    public CharacterView selectedCharacter { get; private set; }
    public event CharacterEvent onCharacterSelected, onCharacterDeselected, onCallChoice;
    public event EventEvent onKnownEventAdded, onKnownEventRemoved;
    public event Delegates.Action onGameOver;
    public int daysLeft = 10;
    public int day { get; private set; }
    public void SetOnTour() {
        onTour = true;
    }

    private bool onTour = false;

    public int actionPointsPerDay = 3;
    public List<Event> meetings, parties;
    public List<Event> knownEvents { get; private set; }

    private bool gameOver = false;
    private int actionPoints;

    private void Awake()
    {
        I = this;

        actionPoints = actionPointsPerDay;
        knownEvents = new List<Event>();
        meetings.ForEach(e => e.eventType = EventType.meeting);
        parties.ForEach(e => e.eventType = EventType.party);
        meetings.Sort((x, y) => x.day - y.day);
        parties.Sort((x, y) => x.day - y.day);
        day = 1;
    }

    private void Update()
    {
        if (gameOver) return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            CharacterView node;
            if (Helpers.ScreenPointToObject(out node, LayerMasks.Node))
            {
                DeselectNode();

                selectedCharacter = node;
                onCharacterSelected(selectedCharacter);
            }
            else
                DeselectNode();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectNode();
        }
    }

    private void DeselectNode()
    {
        if (selectedCharacter != null)
        {
            onCharacterDeselected(selectedCharacter);
        }
    }

    #region public interface
    public void CallSelectedCharacter()
    {
        //answer call
        bool answeredCall = Helpers.Rand(100) < 80;

        if (!answeredCall)
        {
            print("No answer for some reason... I should try again tomorrow.");
        }
        else
        {
            //discussion
            print("Discussion starts!");
            onCallChoice(selectedCharacter);

            actionPoints -= 1;
        }
    }

    public void AddKnownEvent(Event meeting)
    {
        knownEvents.Add(meeting);
        onKnownEventAdded(meeting);
    }

    public Event FindMeeting(CharacterView character, CharacterView character2)
    {
        var ev = meetings.Where(e => e.participants.Contains(character) && e.participants.Contains(character2));
        if (ev.Count() > 0)
            return ev.First();
        return null;
    }

    public Event FindParty(CharacterView character)
    {
        var ev = parties.Where(e => e.participants.Contains(character));
        if (ev.Count() > 0)
            return ev.First();
        return null;
    }

    public void VisitSelectedCharacter()
    {

    }

    public void MessageSelectedCharacter()
    {

    }

    public void CheckDayEnd()
    {
        if (onTour)
        {
            print("Congratz! You got on the tour! You are almost famous...");
            gameOver = true;
            onGameOver();
            return;
        }

        if (actionPoints <= 0)
        {
            day++;
            daysLeft--;


            if (daysLeft == 0)
            {
                print("It is all over now!");
                gameOver = true;
                onGameOver();
            }

            DayStart();
        }
    }

    private void RemoveEvents(List<Event> events)
    {
        for (int i = 0; i < events.Count(); i++)
        {
            var e = events[i];
            if (e.day > day) break;

            if (knownEvents.Contains(e))
            {
                knownEvents.Remove(e);
                onKnownEventRemoved(e);
            }

            events.RemoveAt(i);
            i--;
        }
    }

    private void DayStart()
    {
        actionPoints = actionPointsPerDay;

        {
            var events = knownEvents;
            for (int i = 0; i < events.Count; i++)
            {
                var e = events[i];
                if (e.day > day) break;
                print(string.Format("Went to {0}", e.Name));
                actionPoints--;

                foreach (var p in e.participants)
                {
                    if (p.relationToPlayer == 0) {
                        print(string.Format("Got to know {0} at {1}", p.data.name, e.Name));
                        p.SetRelation(3);
                    }
                }
            }
        }

        //remove used up events
        RemoveEvents(meetings);
        RemoveEvents(parties);


        CheckDayEnd();
    }

    private static string[] dayNames = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    public static string GetDayName(int day)
    {
        return dayNames[(day - 1) % 7];
    }
    #endregion
}
