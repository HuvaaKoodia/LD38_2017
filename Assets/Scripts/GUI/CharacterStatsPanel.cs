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

        panel.SetActive(false);
    }

    private void OpenPanel(CharacterView node)
    {
        panel.SetActive(true);

        buttonsPanel.SetActive(node.relationToPlayer != -1);

        if (node.data.isPlayer)
        {
            stats.text = string.Format("Name:\n{0}\n\nJob:\n{1}\n\nHobbies:\n{2}\n\nTraits:\n{3}",
                node.data.name,
                node.data.job,
                ParseArray(node.data.hobbies),
                ParseList(node.data.personalityTraits)
            );
        }
        else
        {

            stats.text = string.Format("Name:\n{0}\n\nJob:\n{1}\n\nHobbies:\n{2}\n\nTraits:\n{3}\n\nRelation to you:\n{4}",
                node.data.name,
                node.data.job,
                ParseArray(node.data.hobbies),
                ParseList(node.data.personalityTraits),
                CharacterDatabase.I.GetRelationText(node.relationToPlayer)
            );
        }
    }

    private void ClosePanel(CharacterView node)
    {
        panel.SetActive(false);
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
            array[i] = Helpers.Rand(CharacterDatabase.I.personalityTexts[(int)trait]);
        }

        return ParseArray(array);
    }

}
