using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fove.Managed;

public class PositioningObjects : MonoBehaviour {
	GameObject foveInterface;
	void Awake()
	{
		//transform.position, FoveInterface.GetHMDRotation() * Vector3.forward + transform.position
		foveInterface = SceneLoader.instance.foveInterface.GetComponentInChildren<FoveInterface>().gameObject;

		transform.position = FoveInterface.GetHMDRotation() * Vector3.forward *0.55f + foveInterface.transform.position  ;
		transform.LookAt(foveInterface.transform.position);
	}
}
