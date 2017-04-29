using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    public Text textText;
    public float delayTime = 2;
    public bool animationOn { get; private set; }

    private string workingString, fullTextString, targetTextString;
    private int index = 0;
    private float timer;

    void Start()
    {
        Hide();
    }

    public void Show(string name, string text)
    {
        gameObject.SetActive(true);
        workingString = name + "\n\n";
        fullTextString = text;
        targetTextString = workingString + fullTextString;
        index = 0;
        timer = delayTime;

        AudioController.I.PlayAudio(AudioController.I.answerSource);
        animationOn = true;
    }

    public void Hide()
    {
        textText.text = "";
        gameObject.SetActive(false);
        animationOn = false;
    }

    public void ShowTextNow()
    {
        textText.text = targetTextString;
        animationOn = false;
    }

    void Update()
    {
        if (animationOn)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                workingString += fullTextString[index];
                textText.text = workingString;
                index++;
                timer = delayTime;

                if (index == fullTextString.Length)
                {
                    animationOn = false;
                }
            }
        }
    }
}
