using System;
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
    public Transform playerPosition, otherPosition, playerEndPosition;
    public Delegates.Action onDiscussionStart, onDiscussionEnd, onEventChoiceButtons, onEventChosen;
    public DialogPanel playerPanel, otherPanel;
    public Button continueButton, waitButton, restartButton;
    public bool TEST_NoAnswer = false;

    public EventsPanel eventsPanel;

    int actionPointCost = 0;
    int indexDiscussion = 0;
    List<Event> events; 
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
        restartButton.gameObject.SetActive(false);
        waitButton.onClick.AddListener(onWaitButtonPressed);
        restartButton.onClick.AddListener(Helpers.RestartLevel);

        MainController.I.onBeforeFirstDay += ShowIntroDialog;
        MainController.I.onDayStart += CheckWaitButton;
        MainController.I.onDayEnd += OnDayEnd;
        MainController.I.onActionUsed += CheckWaitButton;
        MainController.I.onResolveEvents += onResolveEvents;
        MainController.I.onGameOver += OnGameOver;

        playerCharacter = MainController.I.playerCharacter;
    }
    bool introHack = true;
    private void ShowIntroDialog()
    {
        onDiscussionStart();
        continueButton.gameObject.SetActive(true);
        MovePlayerToDiscussionPosition();
        PlayerTalk("Hey you, I need help with a thing.\n\nAll my life I've wanted to be famous and done nothing about it. Now a world famed celebrity is starting a world tour in just 7 days; it's a ticket to the top.\n\nYou find a way for me talk to them and I'll do the rest, ok?");

    }

    private void OnGameOver()
    {
        if (MainController.I.onTour) {
            otherPanel.Show("Disembodied voice", "Congratz! Ducky got on the tour!\nBy proxy you are almost famous too!");
            AudioController.I.PlayAudio(AudioController.I.victorySource);
            playerCharacter.SetState(DuckState.StarStruck);
        }
        else {
            otherPanel.Show("Disembodied voice", "It is all over now!\nDucky's hopes and dreams, I mean.");
            AudioController.I.PlayAudio(AudioController.I.lifeOverSource);
            playerCharacter.SetState(DuckState.Sad);
            playerCharacter.cryingPS.Play();
        }
        restartButton.gameObject.SetActive(true);

        MovePlayerToEndPosition();
    }

    bool eventHack = false;

    private void onResolveEvents(List<Event> events)
    {
        if (events.Count > 1)
        {
            this.events = events;
            eventHack = true;
            indexDiscussion = 0;
            onDiscussionStart();
            continueButton.gameObject.SetActive(true);
            MovePlayerToDiscussionPosition();
            PlayerTalk(string.Format("Oh noes, I have {0} events on the same day!\nI have to choose one.", events.Count));
 //           AudioController.I.PlayAudio(AudioController.I.answerSource);

        }
        else
        {
            var e = events[0];
            eventHack = false;

            indexDiscussion = 2;

            string text1 = "I went to " + e.GetPlayerDescription() + " with " + e.GetParticipantsList(true) + ".";
            string text2 = "";
            AudioController.I.PlayAudio(e.audiosource);

            var newPeople = new List<CharacterView>();

            foreach (var p in e.participants)
            {
                if (p.relationToPlayer == 0)
                {
                    newPeople.Add(p);
                }
            }

            if (newPeople.Count > 0)
                text2 = "\n" + "I met " + Event.GetCharacterList(newPeople, true) + " for the first time.\nSeems I made a good first impression.";
//            AudioController.I.PlayAudio(AudioController.I.answerSource);
            actionPointCost = 1;

            foreach (var p in newPeople)
            {
                p.SetRelation(3);
            }

            string text3 = Helpers.Rand(new string[] {
            "It was quite an event! You wouldn't believe half the things that went on, so I won't bother divulging any.",
            "It was quite a bit of fun, that!",
            "Would go again, although the food was lacking."
            }
            );
            text3 = "\n\n" + text3;

            onDiscussionStart();
            continueButton.gameObject.SetActive(true);
            MovePlayerToDiscussionPosition();
            PlayerTalk(text1 + text2 + text3);

            MainController.I.RemoveDailyEvents();
        }
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
        if (otherCharacter.data.HasTrait(PersonalityTrait.nice)) relation += 2;
        if (otherCharacter.data.HasTrait(PersonalityTrait.busy)) relation--;
        if (otherCharacter.data.HasTrait(PersonalityTrait.unapproachable)) relation--;

        bool answeredCall = Helpers.RandFloat() < relation / 5f;

        if (otherCharacter.IsScheduleFull(day))
            answeredCall = false;
        otherCharacter.SetSchedule(day, true);//only answers one call per day.. tops.

        if (TEST_NoAnswer) answeredCall = false;

        if (!answeredCall)
        { //No answer
            AudioController.I.PlayAudio(AudioController.I.noAnswerSource);
            onDiscussionStart();
            PlayerTalk("No answer for some reason...\n\nI should try again tomorrow.");
            MovePlayerToDiscussionPosition();
            continueButton.gameObject.SetActive(true);
            indexDiscussion = 2;
            otherCharacter.SetNoAnswer(true);
        }
        else
        {   //Answer
            MoveCharactersToDiscussionPositions();
//            AudioController.I.PlayAudio(AudioController.I.answerSource);
            hackBBC = false;
            if (Helpers.RandPercent() < 5)
            {
                hackBBC = true;
                PlayerTalk("Long time no see.");
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
        MovePlayerToDiscussionPosition();
        otherMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(otherCharacter, otherPosition));
    }

    private void MoveCharactersToNormalPositions()
    {
        if(!MainController.I.GameOver)
            MovePlayerToNormalPosition();
        MoveOtherToNormalPosition();
    }
    
    private void MovePlayerToDiscussionPosition()
    {
        playerMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(playerCharacter, playerPosition));
    }


    private void MovePlayerToEndPosition()
    {
        playerMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(playerCharacter, playerEndPosition));
    }
    private void MoveOtherToNormalPosition()
    {
        if (otherCharacter)
        otherMoverCM.Start(MoveCharacterToDiscussionPositionCoroutine(otherCharacter, otherCharacter.transform));
    }

    private void MovePlayerToNormalPosition()
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
        bool isCeleb = character.connections.Count == 0;//quick hack
        if (isCeleb)
        {
            var button = Instantiate(buttonPrefab, buttonsParent);
            button.GetComponentInChildren<Text>().text = "Can I join the tour, plz?";
            button.onButtonPressedEvent += OnTourButtonPressed;
            button.gameObject.SetActive(true);
            button.character = character;


        }

        //parties?
        if (!isCeleb)
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

    private void OnTourButtonPressed(DiscussionButton button)
    {
        if (otherCharacter.AcceptTourRequestFrom(playerCharacter))
        {
            OtherTalk("Sure. You look stable enough...",
                "Eh, I don't see why not.",
                "You do all the menial work and that is a deal.",
                "I only met you recently and you do seem to have an unhealty obsession with fame, buuuuuut you can't be as bad as the last one, sooooooo you're hired!"
                );
            playerCharacter.SetState(DuckState.Happy);
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
            otherCharacter.SetState(DuckState.Angry);
            playerCharacter.SetState(DuckState.Sad);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnChatButtonPressed(DiscussionButton button)
    {
        OtherTalk(
            "Did you know that water is a rather wet element?\n(Relation +)",
            "Half full or half empty? Seriously, this is why I failed psychology.\n(Relation +)",
            "Everything used to be better back in the day, don't you think?\n\nExcept for Wi-Fi, HD-Video, VR, AR, AC, DC, transcontinental air travel, modern medicine.... toasters.\n(Relation +)",
            "Did you hear, they've finally invented a secure IOT toaster.\nWell, it is big news to me at least!\n(Relation +)",
            "What is on the other side of the pond? I need to know!\n\nWhat do you mean, \"go there\". I can't leave my toaster unguarded!\n(Relation +)"
            );
        otherCharacter.ChangeRelation(1);
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnMeetingButtonPressed(DiscussionButton button)
    {
        CharacterView meetingWith = button.character;
        
        if (otherCharacter.AcceptMeetingRequestFrom(playerCharacter))
        {
            var meeting = MainController.I.FindMeeting(otherCharacter, meetingWith);
            if (meeting != null)
            {
                meeting.invitedBy = otherCharacter;
                OtherTalk("Sure, join " + meeting.GetDescription() + " on " + MainController.GetDayName(meeting.day));
                MainController.I.AddKnownEvent(meeting);
                playerCharacter.SetState(DuckState.Happy);
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
            otherCharacter.SetState(DuckState.Angry);
            playerCharacter.SetState(DuckState.Sad);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

    }

    void OnPartiesButtonPressed(DiscussionButton button)
    {
        if (otherCharacter.AcceptPartyRequestFrom(playerCharacter))
        {
            var party = MainController.I.FindParty(otherCharacter);
            if (party != null)
            {
                party.invitedBy = otherCharacter;
                OtherTalk("Sure, join " + party.GetDescription() + " on " + MainController.GetDayName(party.day));
                MainController.I.AddKnownEvent(party);
                playerCharacter.SetState(DuckState.Happy);
            }
            else
                OtherTalk(
                    "I don't know of any upcoming parties, sorry.",
                    "I haven't been invited to any parties.\nYou don't happen to know of any?",
                    "Sorry no.\n\nThere was a fun one just a few days ago...\nGood food, quality entertainment; we had the time of our lives!\n\nEh.. too bad you weren't invited.",
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
            otherCharacter.SetState(DuckState.Angry);
            playerCharacter.SetState(DuckState.Sad);
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
        if (otherCharacter) otherCharacter.SetTalking(false);
    }

    private void PlayerHideDialogue()
    {
        playerPanel.Hide();
        playerCharacter.SetState(DuckState.Normal);
        playerCharacter.SetTalking(false);
    }

    private void OtherHideDialogue()
    {
        if (!otherCharacter) return;
        otherPanel.Hide();
        otherCharacter.SetTalking(false);
        otherCharacter.SetState(DuckState.Normal);
    }

    public void OnContinueButtonPressed()
    {
        if (introHack) {
            DiscussionEnd();
            return;
        }

        if (eventHack) {
            if (indexDiscussion == 0) {
                ShowEventChoices(events);
                eventsPanel.ShowEventsHack(true);
            }
            if (indexDiscussion == 1)
            {
                OtherHideDialogue();
                MoveOtherToNormalPosition();
                onResolveEvents(events);
                return;
            }

            indexDiscussion++;
            return;
        }

        if (indexDiscussion == 0)
        {
            actionPointCost = 1;
            if (hackBBC)
                OtherTalk("Long time no BBC.");
            else
            {
                if (otherCharacter.relationToPlayer >= 3)
                    OtherTalk("Hi!", "Hello!", "Huldo!");
                else
                    OtherTalk("Oh, it is you again...", "Well well, isn't it my least favorite duck.", "Speak.");

            }
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

    private void ShowEventChoices(List<Event> events)
    {
        continueButton.gameObject.SetActive(false);
        PlayerHideDialogue();

        buttonPanel.SetActive(true);
        //creating buttons
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }

        //meetings?
        int index = 0;
        foreach (var e in events)
        {
            var button = Instantiate(buttonPrefab, buttonsParent);
            button.GetComponentInChildren<Text>().text = "Goto " + e.Name;
            button.onButtonPressedEvent += OnEventButtonPressed;
            button.gameObject.SetActive(true);
            button.Index = index;
            index++;
        }

        onEventChoiceButtons();

    }
    
    private void OnEventButtonPressed(DiscussionButton button)
    {
        eventsPanel.ShowEventsHack(false);
        buttonPanel.gameObject.SetActive(false);
        var selectedEvent = events[button.Index];

        //someone will be pissed! (only 1 for now, lazy!)
        events.Remove(selectedEvent);

        otherCharacter = events[0].invitedBy;

        MoveCharactersToDiscussionPositions();
        otherCharacter.ChangeRelation(-3);
        otherCharacter.SetState(DuckState.Angry);
        OtherTalk(
            "I invited Ducky over and that dastardly cur is nowhere to be seen?\nI will remember this!\n(relation ---)",
            "Quandary: Where is Ducky?\nConclusion: That lame duck!\n(relation ---)",
            "If I invite someone to one of my esteemed events, then they better show up!\n(relation ---)"
            );

        continueButton.gameObject.SetActive(true);
        events.Clear();
        events.Add(selectedEvent);
        onEventChosen();
    }

    private void DiscussionEnd()
    {
        onDiscussionEnd();
        PlayerHideDialogue();
        OtherHideDialogue();
        continueButton.gameObject.SetActive(false);

        if (introHack)
        {
            MainController.I.StartFirstDay();
            introHack = false;
        }
        else
        {
            MainController.I.ReduceActionPoints(actionPointCost);
            MainController.I.CheckDayEnd();
            actionPointCost = 0;
            eventHack = false;
        }

        MoveCharactersToNormalPositions();
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
            if (c.relationToPlayer > 0 && !c.IsScheduleFull(MainController.I.day))
            {
                waitButton.gameObject.SetActive(false);
                return;
            }
        }

        waitButton.gameObject.SetActive(true);
    }

    #endregion
}
