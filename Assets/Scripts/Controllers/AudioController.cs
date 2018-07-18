using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioController I;
    public AudioSource callSource;
    public AudioSource answerSource;
    public AudioSource noAnswerSource;
    public AudioSource nightOutSource;
    public AudioSource morningMeettinSource;
    public AudioSource runnigTripSource;
    public AudioSource prattleSource;
    public AudioSource biographySource;
    public AudioSource teslaSource;
    public AudioSource readingCircleSource;
    public AudioSource partySource;
    public AudioSource partyPreparSource;
    public AudioSource knokingDoorSource;
    public AudioSource doorOpeningSource;
    public AudioSource messageSource;
    public AudioSource victorySource;
    public AudioSource lifeOverSource;
    
    private void Awake() {
        I = this;
    }
    
    public void PlayAudio(AudioSource audio) {
        if (audio != null) {
            audio.Play();
        }
    }
}
