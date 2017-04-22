using UnityEngine;
using System.Collections;

public class SetActiveOnAwake : MonoBehaviour
{
	public bool Active = false;

	void Awake() 
	{
		gameObject.SetActive (Active);
	}
}
