
using UnityEngine;

public class Jitter : MonoBehaviour
{
    public float strength = 1f;

    void Update()
    {
        transform.localPosition = Random.insideUnitSphere * strength;
    }
}
