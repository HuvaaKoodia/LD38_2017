using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public delegate void NodeViewEvent(NodeView planet);

public class NodeView : MonoBehaviour
{
    #region variables
    public Transform graphicsParent;
    public Transform entityParent;
    public List<NodeView> connections;
    public Color lineColor;
    public List<LineView> lines { get; private set; }
    private Dictionary<NodeView, LineView> linesTable;
    private SpriteRenderer mainSprite;

    #endregion
    #region initialization
    private void Awake()
    {
        lines = new List<LineView>();
        linesTable = new Dictionary<NodeView, LineView>();
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
            float lineWidth = 0.4f * pixelsPerUnit;

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
            if (!connection.connections.Contains(this)) {
                connection.connections.Add(this);
            }

            connection.lines.Add(lineView);
            connection.linesTable.Add(this, lineView);
        }
    }

    #endregion
    #region logic
    #endregion
    #region public interface

    public bool ConnectedTo(NodeView currentPlanet)
    {
        return linesTable.ContainsKey(currentPlanet);
    }
    
    public LineView GetLine(NodeView planet)
    {
        return linesTable[planet];
    }

    public void ResetLine(NodeView planet)
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
