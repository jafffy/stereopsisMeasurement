using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public static SceneLoader instance;
	public GameObject foveInterface;
	public GameObject leapMotionPrefab;
	public GameObject interactionManager;
	public GameObject strabismusObjects;
	public float translateTime = 5.0f;
	public string nextSceneName = "Stereoscopsis";

	bool isLoaded = false;
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		if(instance == null) {
			instance = this;
		}
		else {
			Destroy(this);
		}
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

		Destroy(strabismusObjects);

		GameObject foveInterface_s = foveInterface.GetComponentInChildren<FoveInterface>().gameObject;

		GameObject inter = Instantiate(interactionManager);
		inter.transform.SetParent(foveInterface.transform);
		GameObject leap = Instantiate(leapMotionPrefab, foveInterface_s.transform);
	}
}
