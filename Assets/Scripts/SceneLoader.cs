using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public GameObject foveInterface;
	public GameObject leapMotionPrefab;
	public GameObject interactionManager;
	public float translateTime = 5.0f;
	public string nextSceneName = "Stereoscopsis";

	bool isLoaded = false;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		DontDestroyOnLoad(foveInterface);
	}

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

		GameObject inter = Instantiate(interactionManager);
		inter.transform.SetParent(foveInterface.transform);
		GameObject leap = Instantiate(leapMotionPrefab);
		leap.transform.SetParent(foveInterface.transform);
	}
}
