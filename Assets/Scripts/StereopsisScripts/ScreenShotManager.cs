using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotManager : MonoBehaviour {
	public static ScreenShotManager instance; 

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
	public static void MakeScreenShot()
	{
		string location = LoggingManager.GetPath(completePictureName);
		Debug.Log(location);
		ScreenCapture.CaptureScreenshot(location);
	}

}
