using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueControl : MonoBehaviour {

    // Use this for initialization
    void Start() {
        HideContinue();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HideContinue();
        }
    }
    public void ShowContinue() {
        gameObject.SetActive(true);
    }
    public void HideContinue() {
        gameObject.SetActive(false);
    }
}