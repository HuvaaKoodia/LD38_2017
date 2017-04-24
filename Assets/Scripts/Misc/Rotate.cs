using UnityEngine;
public class Rotate : MonoBehaviour {
    public float rotationSpeed;
    public Vector3 axis = Vector3.up;
    public Space space;

	void Update () 
    {
        transform.Rotate(axis, rotationSpeed * Time.deltaTime, space);
	}
}
