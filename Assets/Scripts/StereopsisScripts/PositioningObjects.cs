using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fove.Managed;

public class PositioningObjects : MonoBehaviour {

    const string SceneWithEnvironment = "StereoscopsisWithEnvironment";
    const string SceneWithoutEnvironment = "Stereoscopsis";
    public GameObject globalDirectionalLight;
    public GameObject positionSocket;
    public GameObject environment;

    GameObject foveInterface;

	void Awake()
	{
        foveInterface = SceneLoader.instance.foveInterface.GetComponentInChildren<FoveInterface>().gameObject;
        if(SceneLoader.instance.nextSceneName == SceneWithoutEnvironment)
        {
            AllignWithNoEnvironment();
        }
        else
        {
            AllignWithEnvironment();
        }
	}

    void AllignWithNoEnvironment()
    {
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

    void AllignWithEnvironment()
    {
        AllignWithNoEnvironment();

        Quaternion gazeDir = FoveInterface.GetHMDRotation();
        Matrix4x4 rotTranspose = new Matrix4x4(new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0,
            -positionSocket.transform.rotation.eulerAngles.y + gazeDir.eulerAngles.y,
            0, 1));

        Vector3 newRot = rotTranspose * new Vector4(environment.transform.rotation.eulerAngles.x,
            environment.transform.rotation.eulerAngles.y,
            environment.transform.rotation.eulerAngles.z, 1);
        environment.transform.rotation = Quaternion.Euler(newRot.x, newRot.y, newRot.z);


        Matrix4x4 transpose = new Matrix4x4(new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(- positionSocket.transform.position.x + foveInterface.transform.position.x,
            - positionSocket.transform.position.y + foveInterface.transform.position.y,
            - positionSocket.transform.position.z + foveInterface.transform.position.z, 1));
        Debug.Log(transpose);

        
        Vector3 newPos = transpose * new Vector4(environment.transform.position.x, environment.transform.position.y, environment.transform.position.z , 1);
        Debug.Log(newPos);
        environment.transform.position = newPos;

    }

}
