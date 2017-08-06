using System;
using UnityEngine;
public class Rotate : MonoBehaviour {
    public float rotationSpeed;
    public Vector3 axis = Vector3.up;
    public Space space;

    private Quaternion startLocalRotation;

    private void Awake()
    {
        startLocalRotation = transform.localRotation;
    }

    void Update () 
    {
        transform.Rotate(axis, rotationSpeed * Time.deltaTime, space);
	}

    public void ResetRotation()
    {
        transform.localRotation = startLocalRotation;
    }
}
