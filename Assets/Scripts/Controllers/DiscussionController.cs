﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscussionController : MonoBehaviour
{
    public static DiscussionController I;
    public DiscussionButton buttonPrefab;
    public GameObject buttonPanel;
    public Transform buttonsParent;
    public Transform playerPosition, otherPosition;
    public Delegates.Action onDiscussionStart, onDiscussionEnd;
    public DialogPanel playerPanel, otherPanel;
    public Button continueButton, waitButton;
    public bool TEST_NoAnswer = false;

    int indexDiscussion = 0;

    private CoroutineManager playerMoverCM, otherMoverCM;

    private void Awake()
    {
        I = this;

#if !UNITY_EDITOR
        TEST_NoAnswer = false;
#endif
    }

    private void Start()
    {
        playerMoverCM = new CoroutineManager(this);
        otherMoverCM = new CoroutineManager(this);
        continueButton.gameObject.SetActive(false);
        waitButton.gameObject.SetActive(false);
        waitButton.onClick.AddListener(onWaitButtonPressed);

        MainController.I.onDayStart += CheckWaitButton;
        MainController.I.onDayEnd += OnDayEnd;
        MainController.I.onActionUsed += CheckWaitButton;
        MainController.I.onEventAttended += onEventAttended;
    }

    private void onEventAttended(Event e)
    {
        indexDiscussion = 0;

        var description = e.discussionDescription;
        if (description == "") description = e.Name;

        string text1 = "I went to " + description.Trim() + " with " + e.GetParticipantsList(true) + ".";
        string text2 = "";

        var newPeople = new List<CharacterView>();

        foreach (var p in e.participants)
        {
            if (p.relationToPlayer == 0)
            {
                newPeople.Add(p);
            }
        }

        if (newPeople.Count > 0)
            text2 = "\n" + "I met "+ Event.GetCharacterList(newPeople, true) + " for the first time.\nSeems I made a good first impression.";

        string text3 = Helpers.Rand(new string[] {
            "It was quite an event! You wouldn't believe half the things that went on, so I won't bother divulging any.",
            "It was quite a bit of fun, that!",
            "Would go again, although the food was lacking."
            }
        );
        text3 = "\n\n" + text3;

        continueButton.gameObject.SetActive(true);
        onDiscussionStart();
        MovePlayerToDiscussionPosition();
        PlayerTalk(text1 + text2 + text3);
    }

    bool hackBBC = false;
    #region public interface
    public void CallSelectedCharacter()
    {
        int day = MainController.I.day;

        playerCharacter = MainController.I.playerCharacter;
        otherCharacter = MainController.I.selectedCharacter;

        //answer call
        int relation = otherCharacter.relationToPlayer;

        if (otherCharacter.data.HasTrait(PersonalityTrait.approachable)) relation++;
        if (otherCharacter.data.HasTrait(PersonalityTrait.friendly)) relation++;
        if (otherCharacter.data.HasTrait(PersonalityTrait.busy)) relation--;

        bool answeredCall = Helpers.RandFloat() < relation / 5f;

        if (otherCharacter.IsScheduleFull(day))
            answeredCall = false;
        otherCharacter.SetSchedule(day, true);//only answers one call per day.. tops.

        if (TEST_NoAnswer) answeredCall = false;

        if (!answeredCall)
        {
            onDiscussionStart();
            PlayerTalk("No answer for some reason...\n\nI should try again tomorrow.");
            MovePlayerToDiscussionPosition();
            continueButton.gameObject.SetActive(true);
            indexDiscussion = 2;
        }
        else
        {   //discussion
            MoveCharactersToDiscussionPositions();

            hackBBC = false;
            if (Helpers.RandPercent() < 25)
            {
                hackBBC = true;
                PlayerTalk("Long time no see");
            }
            else
            {
                PlayerTalk(
                    "Hello! It's been a while.",
                    "Good day to you, my \"dearest friend\".",
                    "Hi to you fine madam/sir!"
                    );
            }
            onDiscussionStart();
            MainController.I.ReduceActionPoints(1);
            indexDiscussion = 0;
            continueButton.gameObject.SetActive(true);
        }
    }

    public void VisitSelectedCharacter()
    {

    }

    public void MessageSelectedCharacter()
    {

    }
    #endregion

    #region private interface

    CharacterView playerCharacter, otherCharacter;
    private void MoveCharactersToDiscussionPositions()
    {
        playerMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(playerCharacter, playerPosition));
        otherMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(otherCharacter, otherPosition));
    }

    private void MoveCharactersToNormalPositions()
    {
        playerMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(playerCharacter, playerCharacter.transform));
        otherMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(otherCharacter, otherCharacter.transform));
    }

    private void MovePlayerToDiscussionPosition()
    {
        playerMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(playerCharacter, playerPosition));
    }

    private void MovePlayerToNormalPositions()
    {
        playerMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(playerCharacter, playerCharacter.transform));
    }

    private IEnumerator MoveCharacterToDiscussionPositionCoroutine(CharacterView character, Transform target)
    {
        float percent = 0;
        Vector3 startPosition = character.graphicsParent.position;
        var startRotation = character.graphicsParent.rotation;
        while (percent < 1)
        {
            percent += Time.unscaledDeltaTime * 2f;

            if (percent > 1f) percent = 1f;

            character.graphicsParent.position = Vector3.Lerp(startPosition, target.position, percent);
            character.graphicsParent.rotation = Quaternion.Lerp(startRotation, target.rotation, percent);
            yield return null;
        }
    }

    private void ShowCallChoice(CharacterView character)
    {
        PlayerHideDialogue();
        OtherHideDialogue();
        buttonPanel.SetActive(true);
        //creating buttons
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        //join tour
        bool isCeleb = character.connections.Count == 0;
        if (isCeleb)//quick hack
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
        if (otherCharacter.AcceptTourRequestFrom(playerCharacter))
        {
            OtherTalk("Sure. You look stable enough...",
                "Eh, I don't see why not.",
                "You do all the menial work and that is a deal.",
                "I only met you recently and you do seem to have unhealty obsession with fame.\nYou can't be as bad as the last one.\nYou're hired!"
                );
            MainController.I.SetOnTour();
        }
        else
        {
            OtherTalk(
                "Take you to the tour? I think not! Begone foul shade!\n(Relation --)",
                "Don't make me laugh. Oh well, HAHAHAHAHAAAHAHHAHAHAHA!\n(Relation --)",
                "Ah ha ha ha ha ha ha ha ha ha ha ha ha ha ha oh hee hee ah ha ooh hee ha ha...\nAnd I thought my jokes were bad.\n(Relation --)"
                );
            otherCharacter.ChangeRelation(-2);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnChatButtonPressed(CharacterView character)
    {
        OtherTalk(
            "Did you know that water is a rather wet element?\n(Relation +)",
            "Half full or half empty? Seriously, this is why I failed psychology.\n(Relation +)",
            "Everything used to be better back in the day, don't you think?\n\nExcept for Wi-Fi, HD-Video, VR, AR, AC, DC, transcontinental air travel, modern medicine.... toasters.\n(Relation +)",
            "Did you hear, they've finally invented a secure IOT toaster.\nWell, it is big news for me at least!\n(Relation +)",
            "What is on the other side of the pond? I need to know!\n\nWhat do you mean, \"go there\". I can't leave my toaster unguarded!\n(Relation +)"
            );
        otherCharacter.ChangeRelation(1);
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnMeetingButtonPressed(CharacterView meetingWith)
    {
        if (otherCharacter.AcceptMeetingRequestFrom(playerCharacter))
        {
            var meeting = MainController.I.FindMeeting(otherCharacter, meetingWith);
            if (meeting != null)
            {
                OtherTalk("Sure, join "+meeting.discussionDescription.Trim()+" on " + MainController.GetDayName(meeting.day));
                MainController.I.AddKnownEvent(meeting);
            }
            else
            {
                OtherTalk(
                    "Sure.. Aww, I'm not meeting " + meetingWith.data.name + " anytime soon.\nSorry to get your hopes up.",
                    "Nothing on the schedule with them unfortunately. Maybe some other time.",
                    "Let's see... Nope, can't help you with that.\nThey've blocked me again!"
                    );
            }
        }
        else
        {
            OtherTalk("I'd rather not.\n(Relation -)");
            OtherTalk("I fear that would not be proper.\n(Relation -)");
            OtherTalk("Noooooooooooo *Ahem* ooooooooooooo!.\n(Relation -)");
            otherCharacter.ChangeRelation(-1);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

    }

    void OnPartiesButtonPressed(CharacterView character)
    {
        if (otherCharacter.AcceptPartyRequestFrom(playerCharacter))
        {
            var meeting = MainController.I.FindParty(otherCharacter);
            if (meeting != null)
            {
                OtherTalk("Sure, join us on day " + meeting.day);
                MainController.I.AddKnownEvent(meeting);
            }
            else
                OtherTalk(
                    "I don't know of any upcoming parties, sorry.", 
                    "I haven't been invited to any parties.\nYou don't happen to know of any?",
                    "Sorry no.\n\n There was a fun one just a few days ago...\nGood food, quality entertainment; we had the time of our lives!\n\nEh.. too bad you weren't invited.",
                    "They don't tell me these things no-more.\nI used to be cool, you know."
                    );
        }
        else
        {
            OtherTalk(
                "Hah, our parties are only for respectable enough folk.\n(Relation -)",
                "Naaahhhhhhhhhhh, not for youuuuu.\n(Relation -)",
                "Nope, nope, nope.\n(Relation -)"
                );
            otherCharacter.ChangeRelation(-1);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void PlayerTalk(params string[] texts)
    {
        PlayerTalk(Helpers.Rand(texts));
    }

    private void PlayerTalk(string text)
    {
        OtherStopTalking();
        playerPanel.Show(playerCharacter.data.name, text);
        playerCharacter.SetTalking(true);
    }

    private void OtherTalk(params string[] texts)
    {
        OtherTalk(Helpers.Rand(texts));
    }

    private void OtherTalk(string text)
    {
        PlayerStopTalking();
        otherPanel.Show(otherCharacter.data.name, text);
        otherCharacter.SetTalking(true);
    }

    private void PlayerStopTalking()
    {
        playerCharacter.SetTalking(false);
    }

    private void OtherStopTalking()
    {
        otherCharacter.SetTalking(false);
    }

    private void PlayerHideDialogue()
    {
        playerPanel.Hide();
        playerCharacter.SetTalking(false);
    }

    private void OtherHideDialogue()
    {
        otherPanel.Hide();
        otherCharacter.SetTalking(false);
    }

    public void OnContinueButtonPressed()
    {
        if (indexDiscussion == 0)
        {
            if (hackBBC) 
                OtherTalk("Long time no BBC.");
            else
                OtherTalk("Hi!", "Hello!", "Huldo!");
        }
        if (indexDiscussion == 1)
        {
            PlayerStopTalking();
            OtherStopTalking();
            ShowCallChoice(otherCharacter);
            continueButton.gameObject.SetActive(false);
        }
        if (indexDiscussion == 2)
        {
            DiscussionEnd();
        }
        indexDiscussion++;

    }

    private void DiscussionEnd()
    {
        onDiscussionEnd();
        MoveCharactersToNormalPositions();
        MainController.I.CheckDayEnd();
        PlayerHideDialogue();
        OtherHideDialogue();
        continueButton.gameObject.SetActive(false);
    }

    private void onWaitButtonPressed()
    {
        waitButton.gameObject.SetActive(false);
        MainController.I.ReduceActionPoints(100);
        MainController.I.CheckDayEnd();
    }


    private void OnDayEnd()
    {
        waitButton.gameObject.SetActive(false);
    }

    private void CheckWaitButton()
    {
        //check if there are no talkable characters left and show wait button if that is indeed the case
        foreach (var c in MainController.I.otherCharacters)
        {
            if (c.relationToPlayer != 0 && !c.IsScheduleFull(MainController.I.day))
            {
                waitButton.gameObject.SetActive(false);
                return;
            }
        }

        waitButton.gameObject.SetActive(true);
    }

    #endregion
}
