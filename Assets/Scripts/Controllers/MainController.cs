﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void EventEvent(Event e); //LAL!
public delegate void EventsEvent(List<Event> events); //LEL!

[Serializable]
public class Event
{
    public string Name;
    public string discussionDescription = "";
    public string playerDiscussionDescription = "";
    public int day;
    public CharacterView invitedBy { get; set; }
    public AudioSource audiosource;
    public List<CharacterView> participants;

    public string GetParticipantsList(bool useAnd)
    {
        return GetCharacterList(participants, useAnd);
    }

    public static string GetCharacterList(List<CharacterView> list, bool useAnd)
    {
        var temp = "";
        for (int i = 0; i < list.Count; i++)
        {
            temp += list[i].data.name;
            if (i < list.Count - 1)
            {
                if (useAnd && i == list.Count - 2)
                    temp += " and ";
                else
                    temp += ", ";
            }
        }
        return temp;
    }

    public string GetDescription()
    {
        var description = discussionDescription;
        if (description == "")description = Name;
        return description.Trim();
    }

    public string GetPlayerDescription()
    {
        var description = playerDiscussionDescription;
        if (description == "")description = Name;
        return description.Trim();
    }
}

public class MainController : MonoBehaviour
{
    public static MainController I;

    public CharacterView playerCharacter;
    public List<CharacterView> otherCharacters { get; private set; }
    public CharacterView selectedCharacter { get; private set; }
    public event CharacterEvent onCharacterSelected, onCharacterDeselected;
    public event EventEvent onKnownEventAdded, onKnownEventRemoved;
    public event Delegates.Action onEnd, onActionUsed, onDayStart, onDayEnd, onShowIntro, onIntroStart, onFirstDayStart;
    public int daysLeft = 10;
    public int day { get; private set; }
    public void SetOnTour()
    {
        onTour = true;
    }
    public Color[] lineColors;
    public EventsEvent onResolveEvents;

    public bool onTour { get; private set; }
    public bool GameOver { get { return gameOver; } }

    public GameObject menuPanel;
    public int actionPointsPerDay = 3;
    public List<Event> meetings, parties;
    public List<Event> knownEvents { get; private set; }

    private bool gameOver = false, disableInput = false;
    public int actionPoints { get; private set; }

    private void Awake()
    {
        I = this;

        actionPoints = actionPointsPerDay;
        knownEvents = new List<Event>();
        meetings.Sort((x, y)=> x.day - y.day);
        parties.Sort((x, y)=> x.day - y.day);
        day = 1;
        onTour = false;

        var chars = GameObject.FindGameObjectsWithTag("Character");
        otherCharacters = new List<CharacterView>();
        foreach (var c in chars)
        {
            var character = c.GetComponent<CharacterView>();
            otherCharacters.Add(character);
        }
        otherCharacters.Remove(playerCharacter);
    }

    private IEnumerator Start()
    {
        disableInput = true;
        yield return null;

        //intro setup
        playerCharacter.SetState(DuckState.StarStruck);
        menuPanel.SetActive(true);
        DiscussionController.I.MovePlayerToTitlePosition();
    }
    bool menuOpen = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuOpen)
            {
                Application.Quit();
            }
            else
                Helpers.RestartLevel();
        }

        if (gameOver || disableInput)return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                //skip if over GUI element
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    SelectionInput(touch.position);
            }
        }
#else
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())return; //mouse over GUI element

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectionInput(Input.mousePosition);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectCharacter();
        }
#endif
    }

    #region public interface

    public void ShowIntro()
    {
        menuOpen = false;
        onIntroStart();
        menuPanel.SetActive(false);

        onShowIntro();
    }

    public void StartFirstDay()
    {
        onFirstDayStart();
        onDayEnd();
    }

    public void ReduceActionPoints(int value)
    {
        actionPoints -= value;
        if (onActionUsed != null)onActionUsed();
        UpdateAllLines();
    }

    public void DeselectCharacter()
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.SetSelected(false);
            onCharacterDeselected(selectedCharacter);
        }
    }

    public void AddKnownEvent(Event meeting)
    {
        knownEvents.Add(meeting);
        onKnownEventAdded(meeting);
    }

    public Event FindMeeting(CharacterView character, CharacterView character2)
    {
        var ev = meetings.Where(e => !knownEvents.Contains(e)&& e.participants.Contains(character)&& e.participants.Contains(character2));
        if (ev.Count()> 0)
            return ev.First();
        return null;
    }

    public Event FindParty(CharacterView character)
    {
        var ev = parties.Where(e => !knownEvents.Contains(e)&& e.participants.Contains(character));
        if (ev.Count()> 0)
            return ev.First();
        return null;
    }

    public void DayStart()
    {
        actionPoints = actionPointsPerDay;

        if ((day - 1)% 7 == 0)
            UpdateWeeklySchedules();

        var events = new List<Event>();
        for (int i = 0; i < knownEvents.Count; i++)
        {
            var e = knownEvents[i];
            if (e.day > day)continue;
            events.Add(e);
        }
        if (events.Count > 0)
            onResolveEvents(events);

        if (events.Count == 0)
            RemoveDailyEvents();

        disableInput = false;
        if (onDayStart != null)onDayStart();
    }

    public void UpdateAllLines()
    {
        foreach (var character in otherCharacters)
        {
            character.UpdateLineColours();
        }
    }

    public bool CheckDayEnd()
    {
        if (onTour)
        {
            gameOver = true;
            onEnd();
            return true;
        }

        if (actionPoints <= 0)
        {
            day++;
            daysLeft--;
            actionPoints = actionPointsPerDay;

            //clear uncallable icons
            foreach (var ch in otherCharacters)
            {
                ch.SetNoAnswer(false);
            }

            if (daysLeft == 0)
            {
                gameOver = true;
                onEnd();
                return true;
            }

            DeselectCharacter();
            onDayEnd();
        }

        return false;
    }

    private static string[] dayNames = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    public static string GetDayName(int day)
    {
        return dayNames[(day - 1)% 7];
    }
    #endregion
    #region private interface

    private void SelectionInput(Vector3 screenPosition)
    {
        CharacterView character;
        if (Helpers.ScreenPointToObject(screenPosition, out character, LayerMasks.Character))
        {
            DeselectCharacter();

            selectedCharacter = character;
            selectedCharacter.SetSelected(true);
            onCharacterSelected(selectedCharacter);
        }
        else
            DeselectCharacter();
    }

    private void UpdateWeeklySchedules()
    {
        //clear days
        for (int i = 0; i < otherCharacters.Count; i++)
        {
            otherCharacters[i].schedule = new bool[7];
        }
    }

    public void RemoveDailyEvents()
    {
        RemoveEvents(meetings);
        RemoveEvents(parties);
    }

    private void RemoveEvents(List<Event> events)
    {
        for (int i = 0; i < events.Count(); i++)
        {
            var e = events[i];
            if (e.day > day)break;

            if (knownEvents.Contains(e))
            {
                knownEvents.Remove(e);
                onKnownEventRemoved(e);
            }

            events.RemoveAt(i);
            i--;
        }
    }

    #endregion
}