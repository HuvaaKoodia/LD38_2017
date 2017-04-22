using UnityEngine;
using Vectrosity;

public class LineView : MonoBehaviour 
{
    #region variables
    public CharacterView node1, node2;
    public VectorLine vectorLine;

    public CharacterView GetOther(CharacterView planetView)
    {
        if (planetView == node1) return node2;
        if (planetView == node2) return node1;
        return null;
    }
    #endregion
    #region initialization
    #endregion
    #region logic
    #endregion
    #region public interface
    #endregion
    #region private interface
    #endregion
    #region events
    #endregion
}
