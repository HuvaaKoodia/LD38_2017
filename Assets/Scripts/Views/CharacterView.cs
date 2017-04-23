using UnityEngine;
using System.Collections.Generic;
using Vectrosity;
using System;

public delegate void CharacterEvent(CharacterView node);

public class CharacterView : MonoBehaviour
{
    #region variables
    public string characterId = "id_player";
    public List<CharacterView> connections;
    public Color lineColor;
    public List<LineView> lines { get; private set; }
    private Dictionary<CharacterView, LineView> linesTable;
    public Transform graphicsParent;

    //character stats
    public bool[] schedule { get; set; }

    public CharacterData data { get; private set; }
    public int relationToPlayer { get; private set; }
    public event CharacterEvent onStatsChanged;

    public void ChangeRelation(int change)
    {
        SetRelation(relationToPlayer + change);
    }

    public void SetRelation(int relation)
    {
        relationToPlayer = relation;
        relationToPlayer = Mathf.Clamp(relationToPlayer, 0, 5);

        if (onStatsChanged!= null) onStatsChanged(this);
    }

    public bool IsScheduleFull(int dayIndex)
    {
        return schedule[dayIndex];
    }

    public void SetSchedule(int dayIndex, bool full)
    {
        schedule[dayIndex - 1] = full;
    }

    #endregion
    #region initialization
    private void Awake()
    {
        lines = new List<LineView>();
        linesTable = new Dictionary<CharacterView, LineView>();
    }

    private void Start()
    {
        //load character
        data = CharacterDatabase.I.charactersTable[characterId];
        relationToPlayer = data.relation;

        //create lines
        foreach (var connection in connections)
        {
            if (linesTable.ContainsKey(connection)) continue;

            var direction = (connection.transform.position - transform.position).normalized;
            float targetDistance = 0.3f;

            float worldHeight = Camera.main.orthographicSize * 2;
            float pixelsPerUnit = Camera.main.pixelHeight / worldHeight;
            float lineWidth = 0.2f * pixelsPerUnit;

            var line = new VectorLine("Line",
                new List<Vector3>() {
                    transform.position + direction * targetDistance + Vector3.forward,
                    connection.transform.position - direction * targetDistance + Vector3.forward}
                , lineWidth);

            line.color = lineColor;
            line.collider = true;
            line.layer = LayerMasks.LineIndex;
            line.Draw3D();

            //setup lineView
            var lineView = line.rectTransform.gameObject.AddComponent<LineView>();
            lineView.node1 = this;
            lineView.node2 = connection;
            lineView.vectorLine = line;

            //add to lines
            lines.Add(lineView);
            linesTable.Add(connection, lineView);

            //connect to other node automatically so the line isn't created a new.
            //if (!connection.connections.Contains(this)) {
            //   connection.connections.Add(this);
            //}

            connection.lines.Add(lineView);
            connection.linesTable.Add(this, lineView);
        }
    }

    public bool AcceptTourRequestFrom(CharacterView playerCharacter)
    {
        return Helpers.RandFloat() < relationToPlayer / 5f;
        //return Helpers.RandBool();
    }

    public bool AcceptPartyRequestFrom(CharacterView character)
    {
        int relation = relationToPlayer;
        if (data.HasTrait(PersonalityTrait.helpful)) relation += 1;
        if (data.HasTrait(PersonalityTrait.dishonest)) relation -= 1;

        return Helpers.RandFloat() < relation / 5f;
        //return Helpers.RandBool();
    }

    public bool AcceptMeetingRequestFrom(CharacterView character)
    {
        int relation = relationToPlayer;
        if (data.HasTrait(PersonalityTrait.helpful)) relation += 1;
        if (data.HasTrait(PersonalityTrait.dishonest)) relation -= 1;

        return Helpers.RandFloat() < relation / 5f;
        //return Helpers.RandBool();
    }

    #endregion
    #region logic
    #endregion
    #region public interface

    public bool ConnectedTo(CharacterView currentPlanet)
    {
        return linesTable.ContainsKey(currentPlanet);
    }

    public LineView GetLine(CharacterView planet)
    {
        return linesTable[planet];
    }

    public void ResetLine(CharacterView planet)
    {
        if (linesTable.ContainsKey(planet))
        {
            linesTable[planet].vectorLine.color = lineColor;
        }
    }

    #endregion
    #region private interface

    #endregion
    #region events
    public override string ToString()
    {
        return gameObject.name;
    }
    #endregion
}
