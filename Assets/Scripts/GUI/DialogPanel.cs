using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour {

	public Text textText;
    public string testName = "Player";
    public string testText = "Hello";
    private string temp;
    private string temp2;
    private int index = 0;
    public float delayTime = 2;
    private float timer;
    //public Button continueButton;

    public void Show(string name, string text) {
        gameObject.SetActive(true);
        temp = name + "\n";
        temp2 = text;
        index = 0;

    }

    public void Hide() {
        textText.text = "";
        gameObject.SetActive(false);
    }
	// Use this for initialization
	void Start () {
        Hide();
        
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer < 0 && index < temp2.Length) {
            textText.text = temp + temp2[index];
            temp = temp + temp2[index];
            index++;
            timer = delayTime;
//        if (index == temp2.Length) {
//            continueButton.GetComponent<ContinueControl>().ShowContinue();
        }
    }
}
