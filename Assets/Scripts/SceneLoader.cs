using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public float translateTime = 5.0f;
	public string nextSceneName = "Stereoscopsis";

	bool isLoaded = false;

	void Update()
	{
		if(Time.time > translateTime && !isLoaded)
		{
			LoadAnotherScene();
			isLoaded = true;
		}
	}
	void LoadAnotherScene()
	{	
		Debug.Log("LoadAnotherScnee");
		SceneManager.LoadScene(nextSceneName);
	}
}
