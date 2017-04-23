using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsPanel : MonoBehaviour
{
    public GameObject panel;
    public Text stats;
    public GameObject buttonsPanel;

    private void Start()
    {
        MainController.I.onCharacterSelected += OpenPanel;
        MainController.I.onCharacterDeselected += ClosePanel;
        MainController.I.onGameOver += OnGameover;

        panel.SetActive(false);
    }

    private void OnGameover()
    {
        ClosePanel(null);
    }

    private void OpenPanel(CharacterView character)
    {
        panel.SetActive(true);

        UpdateCharacterStats(character);

        character.onStatsChanged += UpdateCharacterStats;
    }

    private void ClosePanel(CharacterView character)
    {
        panel.SetActive(false);
        if (character != null )
        character.onStatsChanged -= UpdateCharacterStats;
    }

    private void UpdateCharacterStats(CharacterView character)
    {
        buttonsPanel.SetActive(character.relationToPlayer != 0);

        if (character.data.isPlayer)
        {
            stats.text = string.Format("Name:\n{0}\n\nJob:\n{1}\n\nHobbies:\n{2}\n\nTraits:\n{3}",
                character.data.name,
                character.data.job,
                ParseArray(character.data.hobbies),
                ParseList(character.data.personalityTraits)
            );
        }
        else
        {

            stats.text = string.Format("Name:\n{0}\n\nJob:\n{1}\n\nHobbies:\n{2}\n\nTraits:\n{3}\n\nRelation to you:\n{4}",
                character.data.name,
                character.data.job,
                ParseArray(character.data.hobbies),
                ParseList(character.data.personalityTraits),
                CharacterDatabase.I.GetRelationText(character.relationToPlayer)
            );
        }
    }

    public string ParseArray(string[] array)
    {
        if (array.Length == 0) return "None";
        string temp = "";

        for (int i = 0; i < array.Length; i++)
        {
            temp += array[i];
            if (i < array.Length - 1)
            {
                temp += ", ";
            }
        }

        return temp;
    }

    private string ParseList(List<PersonalityTrait> traits)
    {
        string[] array = new string[traits.Count];

        for (int i = 0; i < array.Length; i++)
        {
            var trait = traits[i];
            array[i] = CharacterDatabase.I.personalityTexts[(int)trait][0];//Helpers.Rand(CharacterDatabase.I.personalityTexts[(int)trait])
        }

        return ParseArray(array);
    }
}
