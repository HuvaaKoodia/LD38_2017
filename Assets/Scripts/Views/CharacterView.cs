using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public delegate void CharacterEvent(CharacterView node);
public enum DuckState
{
    Normal = 0,
    Sad,
    Angry,
    Happy,
    StarStruck
}

public class CharacterView : MonoBehaviour
{
    #region variables
    public string characterId = "id_player";
    public ParticleSystem cryingPS;
    public List<CharacterView> connections;
    public List<LineView> lines { get; private set; }
    private Dictionary<CharacterView, LineView> linesTable;
    public Transform graphicsParent;
    public Jitter bodyJitter, eyeJitter;
    public Rotate[] eyeRotators;
    public GameObject selectedSprite, noAnswerSprite;

    //character stats
    public bool[] schedule { get; set; }

    public CharacterData data { get; private set; }
    public int relationToPlayer { get; private set; }
    public event CharacterEvent onStatsChanged;

    public Renderer[] eyeRenderers;
    public Material[] eyeMaterials;  

    public void ChangeRelation(int change)
    {
        SetRelation(relationToPlayer + change);
    }

    public void SetRelation(int relation)
    {
        relationToPlayer = relation;
        relationToPlayer = Mathf.Clamp(relationToPlayer, 1, 5);

        if (onStatsChanged!= null) onStatsChanged(this);
    }

    public bool IsScheduleFull(int dayIndex)
    {
        return schedule[(dayIndex - 1) % 7];
    }

    public void SetSchedule(int dayIndex, bool full)
    {
        schedule[(dayIndex - 1) % 7] = full;
    }

    #endregion
    #region initialization
    private void Awake()
    {
        lines = new List<LineView>();
        linesTable = new Dictionary<CharacterView, LineView>();

        //load character
        data = CharacterDatabase.I.charactersTable[characterId];
        relationToPlayer = data.relation;
    }

    private void Start()
    {
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

            line.color = MainController.I.lineColors[connection.relationToPlayer];
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

        SetSelected(false);
        SetNoAnswer(false);
    }

    DuckState oldState = DuckState.Normal;

    public void SetState(DuckState state)
    {
        eyeRenderers[0].material = eyeMaterials[(int)state];
        eyeRenderers[1].material = eyeMaterials[(int)state];

        if (state == DuckState.Happy)
        {
            animator.SetBool("OpenBeak", true);
            bodyJitter.enabled = true;
        }
        if (oldState == DuckState.Happy)
        {
            animator.SetBool("OpenBeak", false);
            bodyJitter.enabled = false;
        }

        if (state == DuckState.Sad)
        {
            eyeJitter.enabled = true;
        }
        if (oldState == DuckState.Sad)
        {
            eyeJitter.enabled = false;
        }

        if (state == DuckState.StarStruck)
        {
            animator.SetBool("OpenBeak", true);
            bodyJitter.enabled = true;
            eyeRotators[0].enabled = true;
            eyeRotators[1].enabled = true;
        }

        oldState = state;
    }

    public bool AcceptTourRequestFrom(CharacterView playerCharacter)
    {
        return Helpers.RandFloat() < relationToPlayer / 5f;
    }

    public bool AcceptPartyRequestFrom(CharacterView character)
    {
        int relation = relationToPlayer;
        if (data.HasTrait(PersonalityTrait.helpful)) relation += 1;
        if (data.HasTrait(PersonalityTrait.dishonest)) relation -= 1;

        return Helpers.RandFloat() < relation / 5f;
    }

    public bool AcceptMeetingRequestFrom(CharacterView character)
    {
        int relation = relationToPlayer;
        if (data.HasTrait(PersonalityTrait.helpful)) relation += 1;
        if (data.HasTrait(PersonalityTrait.dishonest)) relation -= 1;

        return Helpers.RandFloat() < relation / 5f;
    }

    #endregion
    #region logic
    #endregion
    #region public interface
    public Animator animator;

    public void SetTalking(bool talking) {
        animator.SetBool("Talk", talking);
    }

    public bool ConnectedTo(CharacterView currentPlanet)
    {
        return linesTable.ContainsKey(currentPlanet);
    }

    public LineView GetLine(CharacterView planet)
    {
        return linesTable[planet];
    }

    public void SetSelected(bool selected)
    {
        selectedSprite.gameObject.SetActive(selected);
    }

    public void SetNoAnswer(bool noAnswer)
    {
        noAnswerSprite.gameObject.SetActive(noAnswer);
    }

    public void UpdateLineColours() {
        if (relationToPlayer == 0 && !data.isPlayer) return;
        foreach (var character in connections)
        {
            var line = linesTable[character];

            line.vectorLine.color = MainController.I.lineColors[character.relationToPlayer];
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
