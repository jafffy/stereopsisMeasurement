using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fove.Managed;

public class PositioningObjects : MonoBehaviour {
  public GameObject globalDirectionalLight;

  GameObject foveInterface;

	void Awake()
	{
		foveInterface = SceneLoader.instance.foveInterface.GetComponentInChildren<FoveInterface>().gameObject;

    float forwardAmount = 0.55f;

    transform.position = FoveInterface.GetHMDRotation() * Vector3.forward * forwardAmount;
    transform.position = new Vector3(transform.position.x + foveInterface.transform.position.x,
                                    foveInterface.transform.position.y,
                                    transform.position.z + foveInterface.transform.position.z);
    transform.LookAt(foveInterface.transform.position);

    // Light configuration
    globalDirectionalLight.transform.position = transform.position;
    globalDirectionalLight.transform.rotation = transform.rotation;
    globalDirectionalLight.transform.Rotate(new Vector3(30, 30, 0));
	}
}
