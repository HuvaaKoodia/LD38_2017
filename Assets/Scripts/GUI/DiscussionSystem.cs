using System;
using UnityEngine;
using UnityEngine.UI;

public class DiscussionSystem : MonoBehaviour
{
    public DiscussionButton buttonPrefab;
    public GameObject buttonPanel;
    public Transform buttonsParent;

    void Start()
    {
        MainController.I.onCallChoice += OnCallChoice;
    }

    private void OnCallChoice(CharacterView character)
    {
        buttonPanel.SetActive(true);
        //creating buttons
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        //join tour
        if (character.connections.Count == 0) //quick hack
        {
            var button = Instantiate(buttonPrefab, buttonsParent);
            button.GetComponentInChildren<Text>().text = "Can I join the tour, plz?";
            button.onButtonPressedEvent += OnTourButtonPressed;
            button.gameObject.SetActive(true);
            button.character = character;
        }

        //parties?
        {
            var button = Instantiate(buttonPrefab, buttonsParent);
            button.GetComponentInChildren<Text>().text = "Any parties about?";
            button.onButtonPressedEvent += OnPartiesButtonPressed;
            button.gameObject.SetActive(true);
            button.character = character;
        }

        //meetings?
        foreach (var connectedCharacter in character.connections)
        {
            var button = Instantiate(buttonPrefab, buttonsParent);
            button.GetComponentInChildren<Text>().text = "Arrange meeting with " + connectedCharacter.data.name;
            button.onButtonPressedEvent += OnMeetingButtonPressed;
            button.gameObject.SetActive(true);
            button.character = connectedCharacter;
        }

        //chat
        {
            var button = Instantiate(buttonPrefab, buttonsParent);
            button.GetComponentInChildren<Text>().text = "Chit chat";
            button.onButtonPressedEvent += OnChatButtonPressed;
            button.gameObject.SetActive(true);
            button.character = character;
        }
    }

    private void OnTourButtonPressed(CharacterView node)
    {
        if (MainController.I.selectedCharacter.AcceptTourRequestFrom(MainController.I.playerCharacter))
        {
            print("Sure.. You look stable enough..");
            MainController.I.SetOnTour();
        }
        else
        {
            print("Take you to the tour? I think not! Begone foul shade! (Relation--)");
            MainController.I.selectedCharacter.ChangeRelation(-2);
        }

        DiscussionEnd();
    }

    private void OnChatButtonPressed(CharacterView character)
    {
        print("You chat a bunch. (Relation+)");
        MainController.I.selectedCharacter.ChangeRelation(1);

        DiscussionEnd();
    }

    private void OnMeetingButtonPressed(CharacterView meetingWith)
    {
        if (MainController.I.selectedCharacter.AcceptMeetingRequestFrom(MainController.I.playerCharacter))
        {
            var meeting = MainController.I.FindMeeting(MainController.I.selectedCharacter, meetingWith);
            if (meeting != null)
            {
                print("Sure, join us on day " + meeting.day);
                MainController.I.AddKnownEvent(meeting);
            }
            else
                print("Sure.. Aww, I'm not meeting " + meetingWith.data.name + " anytime soon. Sorry to get your hopes up.");
        }
        else
        {
            print("I'd rather not. (Relation-)");
            MainController.I.selectedCharacter.ChangeRelation(-1);
        }

        DiscussionEnd();
    }

    void OnPartiesButtonPressed(CharacterView character)
    {
        if (MainController.I.selectedCharacter.AcceptPartyRequestFrom(MainController.I.playerCharacter))
        {
            var meeting = MainController.I.FindParty(MainController.I.selectedCharacter);
            if (meeting != null)
            {
                print("Sure, join us on day " + meeting.day);
                MainController.I.AddKnownEvent(meeting);
            }
            else
                print("Don't know of any upcoming parties, sorry.");
        }
        else
        {
            print("Naaahhhhhhhhhhh, not for youuuuu. (Relation-)");
            MainController.I.selectedCharacter.ChangeRelation(-1);
        }

        DiscussionEnd();
    }

    private void DiscussionEnd()
    {
        buttonPanel.gameObject.SetActive(false);
        MainController.I.CheckDayEnd();
    }
}
