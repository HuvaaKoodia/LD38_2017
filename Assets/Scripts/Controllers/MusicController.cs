using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour 
{
    #region variables
    public static MusicController I;

    public AudioMixer mixer;
    public AudioSource menuTrack, mainTrack;
    public AudioMixerSnapshot menuSnapshot, mainSnapshot, noneSnapshot;

    #endregion
    #region initialization

    private void Awake()
    {
        //if (I != null) Destroy(this);

        I = this;
        //GameObject.DontDestroyOnLoad(this);
    }

    private void Start () 
	{
        menuTrack.Play();
        StartSnapshot(menuSnapshot, 0.1f);

        MainController.I.onIntroStart += onStart;
        MainController.I.onEnd += onEnd;
    }

    #endregion
    #region logic
    #endregion
    #region public interface
    #endregion
    #region private interface
    private void StartSnapshot(AudioMixerSnapshot snapshot, float delay = 3)
    {
        mixer.TransitionToSnapshots(new AudioMixerSnapshot[] { snapshot }, new float[] { 1 }, delay);
    }
    #endregion
    #region events

    private void OnEnd()
    {
        StartSnapshot(noneSnapshot);
    }

    private void onStart()
    {
        mainTrack.Play();
        StartSnapshot(mainSnapshot, 3f);
    }

    private void onEnd()
    {
        StartSnapshot(noneSnapshot);
    }
    #endregion
}
