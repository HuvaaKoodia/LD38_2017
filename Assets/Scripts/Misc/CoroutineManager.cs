using System.Collections;
using UnityEngine;

/// <summary>
/// An ease of use wrapper for coroutines.
/// </summary>
public class CoroutineManager
{
    #region vars
    public delegate IEnumerator Enumerator();
    public bool Running { get { return enumerator != null; } }

    private MonoBehaviour parent;
    private IEnumerator enumerator;
    private Enumerator baseEnumerator;
    #endregion
    #region init
    /// <param name="parent">The object where the coroutine is run.</param>
    public CoroutineManager(MonoBehaviour parent)
    {
        this.parent = parent;
    }

    /// <param name="parent">The object where the coroutine is run.</param>
    /// <param name="enumerator">The enumerator run by this manager</param>
    public CoroutineManager(MonoBehaviour parent, Enumerator enumerator)
        : this(parent)
    {
        baseEnumerator = enumerator;
    }
    #endregion
    #region public interface
    /// <summary>
    /// Starts the enumerator set to this manager in the constructor.
    /// Automatically stops previous coroutine execution.
    /// </summary>
    public void Start()
    {
        Start(baseEnumerator());
    }
    /// <summary>
    /// Starts a custom enumerator.
    /// Automatically stops previous coroutine execution.
    /// </summary>
    public void Start(IEnumerator coroutine)
    {
        Stop();
        parent.StartCoroutine(RunCoroutine(coroutine));
    }
    /// <summary>
    /// Stops current coroutine execution.
    /// </summary>
    public void Stop()
    {
        if (enumerator == null) return;
        parent.StopCoroutine(enumerator);
        enumerator = null;
    }
    #endregion
    #region private interface
    private IEnumerator RunCoroutine(IEnumerator coroutine)
    {
        enumerator = coroutine;
        var currentEnumerator = enumerator;
        yield return parent.StartCoroutine(enumerator);
        if(currentEnumerator == enumerator) enumerator = null;
    }
    #endregion
}
