using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoveOrientationDebugChecker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine(transform.position, FoveInterface.GetHMDRotation() * Vector3.forward + transform.position, Color.red, 1.0f );
		//Debug.Log(FoveInterface.GetHMDRotation() * Vector3.forward + transform.position);
	}
}
