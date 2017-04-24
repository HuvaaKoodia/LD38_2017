using UnityEngine;

public class Blink : MonoBehaviour {
    public Renderer eyes;
    public float intervalMin = 2f, intervalMax = 3f;
    public float duration = 0.1f;

    float time = 0;

    private void Start(){
        time = Time.time + Random.Range(intervalMin, intervalMax);
    }

    private void Update(){
        if (time < Time.time) {
            if (eyes.enabled){
                time = Time.time + duration;
            }
            else {
                time = Time.time + Random.Range(intervalMin, intervalMax);
            }
            eyes.enabled = !eyes.enabled;
        }
    }
}
