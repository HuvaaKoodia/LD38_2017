using UnityEngine;

/// <summary>
/// Contains program specific layer bit masks and indices.
/// </summary>
public class LayerMasks
{
    public static readonly int LineIndex = LayerMask.NameToLayer("Line");

    public static readonly int Character = 1 << LayerMask.NameToLayer("Character");
    public static readonly int Line = 1 << LineIndex;
}