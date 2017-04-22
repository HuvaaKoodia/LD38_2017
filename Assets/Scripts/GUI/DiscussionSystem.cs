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

    private void OnChatButtonPressed(CharacterView character)
    {
        print("You chat a bunch. (Relation+)");
        MainController.I.selectedCharacter.ChangeRelation(1);
        buttonPanel.gameObject.SetActive(false);
    }

    private void OnMeetingButtonPressed(CharacterView meetingWith)
    {
        if (MainController.I.selectedCharacter.AcceptMeetingRequestFrom(MainController.I.playerCharacter))
        {
            print("Sure let's meet at ....");
        }
        else
        {
            print("I'd rather not. (Relation-)");
            MainController.I.selectedCharacter.ChangeRelation(-1);
        }

        buttonPanel.gameObject.SetActive(false);
    }

    void OnPartiesButtonPressed(CharacterView character)
    {
        if (MainController.I.selectedCharacter.AcceptPartyRequestFrom(MainController.I.playerCharacter))
        {
            print("Yes there's a party at ...");
        }
        else
        {
            print("Don't know of any upcoming parties, sorry.");
        }

        buttonPanel.gameObject.SetActive(false);
    }
}
