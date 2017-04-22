using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController I;

    public CharacterView playerCharacter;
    public CharacterView selectedCharacter { get; private set; }
    public CharacterEvent onCharacterSelected, onCharacterDeselected, onCallChoice;

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
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
    public void CallSelectedCharacter() {
        //answer call
        bool answeredCall = Helpers.Rand(100) < 80;

        if (!answeredCall)
        {
            print("No answer for some reason... I should try again tomorrow.");
        }
        else {
            print("Discussion starts!");
            onCallChoice(selectedCharacter);
        }

        //discussion

    }

    public void VisitSelectedCharacter()
    {

    }

    public void MessageSelectedCharacter()
    {

    }
    #endregion
}
