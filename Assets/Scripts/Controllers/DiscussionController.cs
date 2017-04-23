using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DiscussionController: MonoBehaviour
{
    public static DiscussionController I;
    public DiscussionButton buttonPrefab;
    public GameObject buttonPanel;
    public Transform buttonsParent;
    public Transform playerPosition, otherPosition;
    public Delegates.Action onDiscussionStart, onDiscussionEnd;
    public DialogPanel playerPanel, otherPanel;
    public Button continueButton;
    int indexDiscussion = 0;

    private CoroutineManager playerMoverCM, otherMoverCM;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        playerMoverCM = new CoroutineManager(this);
        otherMoverCM = new CoroutineManager(this);
        continueButton.gameObject.SetActive(false);
    }

    #region public interface
    public void CallSelectedCharacter()
    {

        var selectedCharacter = MainController.I.selectedCharacter;
        int day = MainController.I.day;

        this.playerCharacter = MainController.I.playerCharacter; 
        this.otherCharacter = MainController.I.selectedCharacter;

        //answer call
        int relation = selectedCharacter.relationToPlayer;

        if (selectedCharacter.data.HasTrait(PersonalityTrait.approachable)) relation++;
        if (selectedCharacter.data.HasTrait(PersonalityTrait.friendly)) relation++;
        if (selectedCharacter.data.HasTrait(PersonalityTrait.busy)) relation--;

        bool answeredCall = Helpers.RandFloat() < relation / 5f;

        if (selectedCharacter.IsScheduleFull(day)) answeredCall = false;
        selectedCharacter.SetSchedule(day, true);//only answers one call per day.. tops.
        if (!answeredCall) {
            playerPanel.Show(playerCharacter.data.name, "No answer for some reason... I should try again tomorrow.");
            continueButton.gameObject.SetActive(true);
            indexDiscussion = 2;
        }
        else
        {   //discussion
            MoveCharactersToDiscussionPositions();
            playerPanel.Show(playerCharacter.data.name, "Hello!");
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
        playerPanel.Hide();
        otherPanel.Hide();
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
            //print("Sure. You look stable enough...");
            otherPanel.Show(otherCharacter.data.name, "Sure. You look stable enough...");
            MainController.I.SetOnTour();
        }
        else
        {
            //print("Take you to the tour? I think not! Begone foul shade! (Relation--)");
            otherPanel.Show(otherCharacter.data.name, "Take you to the tour? I think not! Begone foul shade! "/*(Relation--)*/);
            MainController.I.selectedCharacter.ChangeRelation(-2);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnChatButtonPressed(CharacterView character)
    {
        //print("You chat a bunch. (Relation+)");
        otherPanel.Show(otherCharacter.data.name, "You chat a bunch. "/*(Relation+)*/);
        MainController.I.selectedCharacter.ChangeRelation(1);
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    private void OnMeetingButtonPressed(CharacterView meetingWith)
    {
        if (MainController.I.selectedCharacter.AcceptMeetingRequestFrom(MainController.I.playerCharacter))
        {
            var meeting = MainController.I.FindMeeting(MainController.I.selectedCharacter, meetingWith);
            if (meeting != null) {
                otherPanel.Show(otherCharacter.data.name, "Sure, join us on day " + meeting.day);
                MainController.I.AddKnownEvent(meeting);
            }
            else {
                otherPanel.Show(otherCharacter.data.name, "Sure.. Aww, I'm not meeting " + meetingWith.data.name + " anytime soon. Sorry to get your hopes up.");
            }
        }
        else
        {
            otherPanel.Show(otherCharacter.data.name, "I'd rather not. "/*(Relation-)*/);
            MainController.I.selectedCharacter.ChangeRelation(-1);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);

    }

    void OnPartiesButtonPressed(CharacterView character)
    {
        if (MainController.I.selectedCharacter.AcceptPartyRequestFrom(MainController.I.playerCharacter))
        {
            var meeting = MainController.I.FindParty(MainController.I.selectedCharacter);
            if (meeting != null) {
                //print("Sure, join us on day " + meeting.day);
                otherPanel.Show(otherCharacter.data.name, "Sure, join us on day " + meeting.day);
                MainController.I.AddKnownEvent(meeting);
            }
            else
                otherPanel.Show(otherCharacter.data.name, "Don't know of any upcoming parties, sorry.");
        }
        else
        {
            //print("Naaahhhhhhhhhhh, not for youuuuu. (Relation-)");
            otherPanel.Show(otherCharacter.data.name, "Naaahhhhhhhhhhh, not for youuuuu. (Relation-)");
            MainController.I.selectedCharacter.ChangeRelation(-1);
        }
        buttonPanel.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }
    public void OnContinueButtonPressed() {
        if (indexDiscussion == 0) {
            otherPanel.Show(otherCharacter.data.name, "Hi");
        }
        if (indexDiscussion == 1) {
            ShowCallChoice(otherCharacter);
            continueButton.gameObject.SetActive(false);
        }
        if (indexDiscussion == 2) {
            DiscussionEnd();
        }
        indexDiscussion++;

    }

    private void DiscussionEnd()
    {

        onDiscussionEnd();
        MoveCharactersToNormalPositions();
        MainController.I.CheckDayEnd();
        playerPanel.Hide();
        otherPanel.Hide();
        continueButton.gameObject.SetActive(false);
    }
    #endregion
}
