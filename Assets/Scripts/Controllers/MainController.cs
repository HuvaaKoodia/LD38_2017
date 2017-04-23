using System;
using System.Collections;
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
    public List<CharacterView> otherCharacters { get; private set; }
    public CharacterView selectedCharacter { get; private set; }
    public event CharacterEvent onCharacterSelected, onCharacterDeselected;
    public event EventEvent onKnownEventAdded, onKnownEventRemoved;
    public event Delegates.Action onGameOver, onActionUsed, onDayStart;
    public int daysLeft = 10;
    public int day { get; private set; }
    public void SetOnTour() {
        onTour = true;
    }

    private bool onTour = false;

    public int actionPointsPerDay = 3;
    public List<Event> meetings, parties;
    public List<Event> knownEvents { get; private set; }

    private bool gameOver = false, disableInput = false;
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

    public void ReduceActionPoints(int value)
    {
        actionPoints -= value;
        if (onActionUsed != null) onActionUsed();
    }

    private IEnumerator Start()
    {
        var chars = GameObject.FindGameObjectsWithTag("Character");
        otherCharacters = new List<CharacterView>();
        foreach (var c in chars)
        {
            var character = c.GetComponent<CharacterView>();
            otherCharacters.Add(character);
        }
        otherCharacters.Remove(playerCharacter);

        disableInput = true;
        yield return null;
        DayStart();
    }

    private void Update()
    {
        if (gameOver && disableInput) return;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;//mouse over GUI element

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CharacterView node;
            if (Helpers.ScreenPointToObject(out node, LayerMasks.Character))
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

    public bool CheckDayEnd()
    {
        if (onTour)
        {
            print("Congratz! You got on the tour! You are almost famous!");
            gameOver = true;
            onGameOver();
            return true;
        }

        if (actionPoints <= 0)
        {
            day++;
            daysLeft--;

            if (daysLeft == 1)
            {
                print("It is all over now! Your life I mean.");
                gameOver = true;
                onGameOver();
                return true;
            }

            DayStart();
        }

        return false;
    }

    private static string[] dayNames = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    public static string GetDayName(int day)
    {
        return dayNames[(day - 1) % 7];
    }
    #endregion

    #region private interface

    private void UpdateWeeklySchedules()
    {
        //clear days
        for (int i = 0; i < otherCharacters.Count; i++)
        {
            otherCharacters[i].schedule = new bool[7];
        }

        //set events
        var events = parties;
        for (int i = 0; i < otherCharacters.Count; i++)
        {
            foreach (var e in events)
            {
                if (e.participants.Contains(otherCharacters[i])){
                    otherCharacters[i].SetSchedule(e.day, true);
                    break;
                }
            }
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

        if ((day - 1) % 7 == 0)
            UpdateWeeklySchedules();

        {
            var events = knownEvents;
            for (int i = 0; i < events.Count; i++)
            {
                var e = events[i];
                if (e.day > day) continue;
                print(string.Format("Went to {0}", e.Name));
                actionPoints--;

                foreach (var p in e.participants)
                {
                    if (p.relationToPlayer == 0)
                    {
                        print(string.Format("Got to know {0} at {1}", p.data.name, e.Name));
                        p.SetRelation(3);
                    }
                }
            }
        }

        //remove used up events
        RemoveEvents(meetings);
        RemoveEvents(parties);

        if (!CheckDayEnd())
        {
            disableInput = false;
            if (onDayStart != null) onDayStart();
        }
    }

    #endregion
}
