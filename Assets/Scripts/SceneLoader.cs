using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public float translateTime = 40.0f;
	public string nextSceneName = "littelTweakOnBoxScene";

	void Update()
	{
		if(Time.deltaTime > translateTime)
		{
			LoadAnotherScene();
		}
	}
	void LoadAnotherScene()
	{
		SceneManager.LoadScene(nextSceneName);
	}
}
