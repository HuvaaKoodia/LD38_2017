using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PersonalityTrait
{
    approachable = 0,
    dishonest,
    helpful,
    caring,
    friendly,
    busy,
    efficient,
    unapproachable,
    nice,
    obsessive,
    compulsive,
    amount
}

public class CharacterData
{
    public string name;
    public string job;
    public string[] hobbies;
    public List<PersonalityTrait> personalityTraits;
    public int relation;
    public bool isPlayer;

    public bool HasTrait(PersonalityTrait trait)
    {
        return personalityTraits.Contains(trait);
    }
}

public class CharacterDatabase : MonoBehaviour
{
    public static CharacterDatabase I;
    public TextAsset people, personalities;
    public Dictionary<string, CharacterData> charactersTable;
    public List<string>[] personalityTexts;

    private void Awake()
    {
        I = this;

        //parse data
        {
            charactersTable = new Dictionary<string, CharacterData>();

            var rows = people.text.Split('\n');
            for (int i = 2; i < rows.Length; i++)
            {
                var data = new CharacterData();

                var columns = Helpers.Split(rows[i], ";", false);
                string id = columns[0];

                if (string.IsNullOrEmpty(id)) continue;
                data.name = columns[1];
                data.job = columns[2];
                data.hobbies = Helpers.Split(columns[3], ",", true, true);

                {//traits
                    var split = Helpers.Split(columns[4], ",", true, true);

                    var list = new List<PersonalityTrait>();
                    foreach (var trait in split)
                    {
                        var traitEnum = ParseTrait(trait);

                        list.Add(traitEnum);
                    }

                    data.personalityTraits = list;
                }


                data.relation = int.Parse(columns[5]);

                data.isPlayer = id == "id_player";
                charactersTable.Add(id, data);
            }
        }

        {
            personalityTexts = new List<string>[(int)PersonalityTrait.amount];

            var rows = personalities.text.Split('\n');
            for (int i = 2; i < rows.Length; i++)
            {
                var columns = Helpers.Split(rows[i], ";", false);
                if (string.IsNullOrEmpty(columns[0])) continue;
                PersonalityTrait idEnum = ParseTrait(columns[0]);

                var options = Helpers.Split(columns[1], ",", false, true);

                personalityTexts[(int)idEnum] = options.ToList();

            }
        }
    }

    private PersonalityTrait ParseTrait(string trait)
    {
        PersonalityTrait idEnum = PersonalityTrait.amount;
        try
        {
            idEnum = (PersonalityTrait)Enum.Parse(typeof(PersonalityTrait), trait);
        }
        catch (Exception e)
        {
            e.GetType();//just to suppress the warning....
            Debug.LogErrorFormat("{0} is not a valid personality type!", trait);
        }
        return idEnum;
    }

    public string GetRelationText(int relation)
    {
        if (relation == 0) return "Stranger";

        if (relation == 5) return "Splendid!";
        if (relation == 4) return "Good";
        if (relation == 3) return "Neutral";
        if (relation == 2) return "Dislike";
        return "Hate";
    }
}
