using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotManager : MonoBehaviour {
	public static ScreenShotManager instance; 
	[Tooltip("Define How often unity captures screen automatically")]
	public float autoCaptureInterval = 5.0f;
	float timeBucket = 0;
	static string completePictureName = "completePicture.png";
	void Awake()
	{
		if(instance == null) {
			instance = this;
		}
		else {
			Destroy(this);
		}
	}
	void Update()
	{
		if(Time.time > timeBucket + autoCaptureInterval)
		{
			timeBucket = Time.time;
			MakeScreenShot();
		}
	}
	public static void MakeScreenShot()
	{
		string location = LoggingManager.GetPath(completePictureName);
		Debug.Log(location);
		ScreenCapture.CaptureScreenshot(location);
	}

}
