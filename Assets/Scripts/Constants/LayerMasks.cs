using UnityEngine;

/// <summary>
/// Contains program specific layer bit masks and indices.
/// </summary>
public class LayerMasks
{
    public static readonly int LineIndex = LayerMask.NameToLayer("Line");

    public static readonly int Node = 1 << LayerMask.NameToLayer("Node");
    public static readonly int Line = 1 << LineIndex;
}