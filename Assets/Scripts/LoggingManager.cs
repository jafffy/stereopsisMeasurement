using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingManager : MonoBehaviour {

	public static LoggingManager instance;
	
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
}
