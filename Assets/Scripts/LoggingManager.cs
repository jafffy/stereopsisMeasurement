using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingManager : MonoBehaviour {

	public static LoggingManager instance;
	[Tooltip("If this is false, Data file will be located on Desktop")]
	public bool locateDataFileOnAssets = false;
	public string patientID;
	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);   
		} 
		DontDestroyOnLoad(gameObject);
		if(!IsValid(patientID))
		{
			Debug.LogError("PatientID를 확인해 주세요");
		}
	}

	bool IsValid(string id)
	{
		if(id != null && id.Length != 0) { 
			return true;
		}
		return false;
	}
	static public string GetPath(string subfix)
	{
		return instance.locateDataFileOnAssets ? GetAssetsPath(subfix) : GetDesktopPath(subfix);
	}
	static string GetAssetsPath(string subfix)
	{
		return "Assets/" + instance.patientID+ "/" + subfix;
	}
	static string GetDesktopPath(string subfix)
	{
		return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)
			+ "/"+instance.patientID+ "/" + subfix;
	}
}
